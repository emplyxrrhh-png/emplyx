Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Absence
Imports Robotics.Base.VTBusiness.Common

Public Class ProgrammedAbsencesMethods

    Public Shared Function GetProgrammedAbsences(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim bState = New roProgrammedAbsenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim ds As New DataSet
        Dim tb As DataTable = roProgrammedAbsence.GetProgrammedAbsences(IDEmployee, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetProgrammedAbsencesInPeriod(ByVal IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _EndDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim bState = New roProgrammedAbsenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim ds As New DataSet
        Dim tb As DataTable = roProgrammedAbsence.GetProgrammedAbsences(IDEmployee, _BeginDate, _EndDate, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetProgrammedAbsence(ByVal IDEmployee As Integer, ByVal BeginDate As Date, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roProgrammedAbsence)

        Dim oResult As New roGenericVtResponse(Of roProgrammedAbsence)
        Dim bState = New roProgrammedAbsenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roProgrammedAbsence.GetProgrammedAbsence(IDEmployee, BeginDate, bState, bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function ValidateProgrammedAbsence(ByVal ProgrammedAbsence As roProgrammedAbsence, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roProgrammedAbsenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roProgrammedAbsence.ValidateProgrammedAbsence(ProgrammedAbsence, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveProgrammedAbsence(ByVal ProgrammedAbsence As roProgrammedAbsence, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roProgrammedAbsence)

        Dim oResult As New roGenericVtResponse(Of roProgrammedAbsence)
        Dim bState = New roProgrammedAbsenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        ProgrammedAbsence.State = bState

        If ProgrammedAbsence.Save(bolAudit) Then
            oResult.Value = ProgrammedAbsence
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function DeleteProgrammedAbsence(ByVal ProgrammedAbsence As roProgrammedAbsence, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roProgrammedAbsenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        ProgrammedAbsence.State = bState
        oResult.Value = ProgrammedAbsence.Delete(bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetProgrammedCauses(ByVal IDEmployee As Integer, ByVal xBegin As Date, ByVal xEnd As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim bState = New roProgrammedAbsenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim ds As New DataSet
        Dim tb As DataTable = roProgrammedAbsence.GetProgrammedCauses(IDEmployee, xBegin, xEnd, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

End Class