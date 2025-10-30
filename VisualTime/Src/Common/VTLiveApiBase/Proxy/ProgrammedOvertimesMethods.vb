Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTHolidays

Public Class ProgrammedOvertimesMethods

    Public Shared Function GetProgrammedOvertimeById(ByVal idProgrammedOvertime As Long, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProgrammedOvertimeResponse
        Dim bState = New roProgrammedOvertimeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oProgrammedOvertimeManager As New roProgrammedOvertimeManager(bState)
        Dim oProgrammedOvertime = oProgrammedOvertimeManager.LoadProgrammedOvertime(idProgrammedOvertime)

        'crear el response genérico
        Dim genericResponse As New roProgrammedOvertimeResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProgrammedOvertimeManager.State, newGState)
        genericResponse.ProgrammedOvertime = oProgrammedOvertime
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetProgrammedOvertimes(ByVal idEmployee As Integer, ByVal strWhere As String, ByVal oState As roWsState) As roProgrammedOvertimeListResponse

        Dim bState = New roProgrammedOvertimeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oProgrammedOvertimeManager As New roProgrammedOvertimeManager(bState)
        Dim oProgrammedOvertime As Generic.List(Of roProgrammedOvertime) = oProgrammedOvertimeManager.GetProgrammedOvertimes(idEmployee, bState, strWhere)

        'crear el response genérico
        Dim genericResponse As New roProgrammedOvertimeListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProgrammedOvertimeManager.State, newGState)
        genericResponse.ProgrammedOvertimes = If(oProgrammedOvertime IsNot Nothing, oProgrammedOvertime.ToArray, {})
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function SaveProgrammedOvertime(ByVal oProgrammedOvertime As roProgrammedOvertime, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProgrammedOvertimeResponse

        Dim bState = New roProgrammedOvertimeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oProgrammedOvertimeManager As New roProgrammedOvertimeManager(bState)
        Dim oId As Integer = oProgrammedOvertimeManager.SaveProgrammedOvertime(oProgrammedOvertime, bAudit)

        If (oId > 0) Then oProgrammedOvertime.ID = oId
        'crear el response genérico
        Dim genericResponse As New roProgrammedOvertimeResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProgrammedOvertimeManager.State, newGState)
        genericResponse.ProgrammedOvertime = oProgrammedOvertime
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function DeleteProgrammedOvertime(ByVal oProgrammedOvertime As roProgrammedOvertime, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProgrammedOvertimeStandarResponse
        Dim bState = New roProgrammedOvertimeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oProgrammedOvertimeManager As New roProgrammedOvertimeManager(bState)
        Dim oRet = oProgrammedOvertimeManager.DeleteProgrammedOvertime(oProgrammedOvertime, bAudit)

        'crear el response genérico
        Dim genericResponse As New roProgrammedOvertimeStandarResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProgrammedOvertimeManager.State, newGState)
        genericResponse.Result = oRet
        genericResponse.oState = newGState
        Return genericResponse
    End Function

End Class