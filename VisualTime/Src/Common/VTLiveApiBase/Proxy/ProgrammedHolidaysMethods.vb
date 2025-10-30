Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTHolidays

Public Class ProgrammedHolidaysMethods

    Public Shared Function GetProgrammedHolidayById(ByVal idProgrammedHoliday As Long, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProgrammedHolidayResponse
        Dim bState = New roProgrammedHolidayState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oProgrammedHolidayManager As New roProgrammedHolidayManager(bState)
        Dim oProgrammedHoliday = oProgrammedHolidayManager.LoadProgrammedHoliday(idProgrammedHoliday)

        'crear el response genérico
        Dim genericResponse As New roProgrammedHolidayResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProgrammedHolidayManager.State, newGState)
        genericResponse.ProgrammedHoliday = oProgrammedHoliday
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetProgrammedHolidays(ByVal idEmployee As Integer, ByVal strWhere As String, ByVal oState As roWsState) As roProgrammedHolidayListResponse

        Dim bState = New roProgrammedHolidayState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oProgrammedHolidayManager As New roProgrammedHolidayManager(bState)
        Dim oProgrammedHoliday As Generic.List(Of roProgrammedHoliday) = oProgrammedHolidayManager.GetProgrammedHolidays(idEmployee, bState, strWhere)

        'crear el response genérico
        Dim genericResponse As New roProgrammedHolidayListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProgrammedHolidayManager.State, newGState)
        genericResponse.ProgrammedHolidays = If(oProgrammedHoliday IsNot Nothing, oProgrammedHoliday.ToArray, {})
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function SaveProgrammedHoliday(ByVal oProgrammedHoliday As roProgrammedHoliday, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProgrammedHolidayResponse

        Dim bState = New roProgrammedHolidayState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oProgrammedHolidayManager As New roProgrammedHolidayManager(bState)
        Dim oId As Integer = oProgrammedHolidayManager.SaveProgrammedHoliday(oProgrammedHoliday, bAudit)

        If (oId > 0) Then oProgrammedHoliday.ID = oId
        'crear el response genérico
        Dim genericResponse As New roProgrammedHolidayResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProgrammedHolidayManager.State, newGState)
        genericResponse.ProgrammedHoliday = oProgrammedHoliday
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function DeleteProgrammedHoliday(ByVal oProgrammedHoliday As roProgrammedHoliday, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProgrammedHolidayStandarResponse

        Dim bState = New roProgrammedHolidayState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oProgrammedHolidayManager As New roProgrammedHolidayManager(bState)
        Dim oRet = oProgrammedHolidayManager.DeleteProgrammedHoliday(oProgrammedHoliday, bAudit)

        'crear el response genérico
        Dim genericResponse As New roProgrammedHolidayStandarResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProgrammedHolidayManager.State, newGState)
        genericResponse.Result = oRet
        genericResponse.oState = newGState
        Return genericResponse
    End Function

End Class