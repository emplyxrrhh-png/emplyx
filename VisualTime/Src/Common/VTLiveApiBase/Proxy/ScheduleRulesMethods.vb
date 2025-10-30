Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTCalendar

Public Class ScheduleRulesMethods

    Public Shared Function GetLabAgreeScheduleRules(ByVal IdLabAgree As Integer, ByVal oState As roWsState) As roScheduleRulesResponse
        Dim bState = New roCalendarScheduleRulesState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oTmpLst As New Generic.List(Of roScheduleRule)
        Dim oScheduleRulesManager As New roCalendarScheduleRulesManager(bState)
        oTmpLst = oScheduleRulesManager.GetScheduleRules(IdLabAgree)

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oScheduleRulesManager.State, newGState)

        Dim genericResponse As New roScheduleRulesResponse
        genericResponse.Rules = oTmpLst.ToArray
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetContractScheduleRules(ByVal IdContract As String, ByVal oState As roWsState) As roScheduleRulesResponse
        Dim bState = New roCalendarScheduleRulesState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oTmpLst As New Generic.List(Of roScheduleRule)
        Dim oScheduleRulesManager As New roCalendarScheduleRulesManager(bState)
        oTmpLst = oScheduleRulesManager.GetScheduleRules(, IdContract)

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oScheduleRulesManager.State, newGState)

        Dim genericResponse As New roScheduleRulesResponse
        genericResponse.Rules = oTmpLst.ToArray
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetEmployeeCurrentScheduleRules(ByVal IdContract As String, ByVal oState As roWsState) As roScheduleRulesResponse
        Dim bState = New roCalendarScheduleRulesState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oTmpLst As New Generic.List(Of roScheduleRule)
        Dim oScheduleRulesManager As New roCalendarScheduleRulesManager(bState)
        oTmpLst = oScheduleRulesManager.GetEmployeeCurrentScheduleRules(IdContract)

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oScheduleRulesManager.State, newGState)

        Dim genericResponse As New roScheduleRulesResponse
        genericResponse.Rules = oTmpLst.ToArray
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function SaveLabAgreeScheduleRules(ByVal IdLabAgree As Integer, ByVal oScheduleRules As roScheduleRule(), ByVal oState As roWsState) As roScheduleRulesStdResponse
        Dim bState = New roCalendarScheduleRulesState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oTmpLst As New Generic.List(Of roScheduleRule)
        Dim bRes As Boolean = False
        Dim oScheduleRulesManager As New roCalendarScheduleRulesManager(bState)
        bRes = oScheduleRulesManager.SaveScheduleRules(oScheduleRules, IdLabAgree)

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oScheduleRulesManager.State, newGState)

        Dim genericResponse As New roScheduleRulesStdResponse
        genericResponse.Result = bRes
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function SaveContractScheduleRules(ByVal IdContract As String, ByVal oScheduleRules As roScheduleRule(), ByVal oState As roWsState) As roScheduleRulesStdResponse
        Dim bState = New roCalendarScheduleRulesState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()
        Dim bRes As Boolean = False
        Dim oScheduleRulesManager As New roCalendarScheduleRulesManager(bState)
        bRes = oScheduleRulesManager.SaveScheduleRules(oScheduleRules,, IdContract)

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oScheduleRulesManager.State, newGState)

        Dim genericResponse As New roScheduleRulesStdResponse
        genericResponse.Result = bRes
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetUserScheduleRulesTypes(ByVal oState As roWsState) As roScheduleRulesTypesResponse
        Dim bState = New roCalendarScheduleRulesState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()
        Dim bRes As Generic.List(Of Integer)
        Dim oScheduleRulesManager As New roCalendarScheduleRulesManager(bState)
        bRes = oScheduleRulesManager.GetUserScheduleRulesTypes()

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oScheduleRulesManager.State, newGState)

        Dim genericResponse As New roScheduleRulesTypesResponse
        genericResponse.Rules = bRes.ToArray
        genericResponse.oState = newGState

        Return genericResponse
    End Function

End Class