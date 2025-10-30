Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Incidence

Public Class ProgrammedCausesMethods

    Public Shared Function GetProgrammedCauses(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim bState = New roProgrammedCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim ds As New DataSet
        Dim tb As DataTable = roProgrammedCause.GetProgrammedCauses(IDEmployee, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetProgrammedCause(ByVal IDEmployee As Integer, ByVal ProgrammedDate As Date, ByVal IDAbsence As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roProgrammedCause)

        Dim oResult As New roGenericVtResponse(Of roProgrammedCause)
        Dim bState = New roProgrammedCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roProgrammedCause.GetProgrammedCause(IDEmployee, ProgrammedDate, IDAbsence, bState, bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function ValidateProgrammedCause(ByVal ProgrammedCause As roProgrammedCause, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roProgrammedCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roProgrammedCause.ValidateProgrammedCause(ProgrammedCause, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function SaveProgrammedCause(ByVal ProgrammedCause As roProgrammedCause, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roProgrammedCause)

        Dim oResult As New roGenericVtResponse(Of roProgrammedCause)

        Dim bState = New roProgrammedCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        ProgrammedCause.State = bState

        If ProgrammedCause.Save(bolAudit) Then
            oResult.Value = ProgrammedCause
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function DeleteProgrammedCause(ByVal ProgrammedCause As roProgrammedCause, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roProgrammedCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        ProgrammedCause.State = bState
        oResult.Value = ProgrammedCause.Delete(bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

End Class