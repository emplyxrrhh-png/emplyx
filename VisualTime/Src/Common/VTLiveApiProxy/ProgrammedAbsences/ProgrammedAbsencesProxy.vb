Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Absence

Public Class ProgrammedAbsencesProxy
    Implements IProgrammedAbsencesSvc

    Public Function KeepAlive() As Boolean Implements IProgrammedAbsencesSvc.KeepAlive
        Return True
    End Function

    Public Function GetProgrammedAbsences(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IProgrammedAbsencesSvc.GetProgrammedAbsences
        Return ProgrammedAbsencesMethods.GetProgrammedAbsences(IDEmployee, oState)
    End Function
    Public Function GetProgrammedAbsencesInPeriod(ByVal IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _EndDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IProgrammedAbsencesSvc.GetProgrammedAbsencesInPeriod
        Return ProgrammedAbsencesMethods.GetProgrammedAbsencesInPeriod(IDEmployee, _BeginDate, _EndDate, oState)
    End Function
    Public Function GetProgrammedAbsence(ByVal IDEmployee As Integer, ByVal BeginDate As Date, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roProgrammedAbsence) Implements IProgrammedAbsencesSvc.GetProgrammedAbsence
        Return ProgrammedAbsencesMethods.GetProgrammedAbsence(IDEmployee, BeginDate, oState, bolAudit)
    End Function

    Public Function ValidateProgrammedAbsence(ByVal ProgrammedAbsence As roProgrammedAbsence, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IProgrammedAbsencesSvc.ValidateProgrammedAbsence
        Return ProgrammedAbsencesMethods.ValidateProgrammedAbsence(ProgrammedAbsence, oState)
    End Function

    Public Function SaveProgrammedAbsence(ByVal ProgrammedAbsence As roProgrammedAbsence, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roProgrammedAbsence) Implements IProgrammedAbsencesSvc.SaveProgrammedAbsence
        Return ProgrammedAbsencesMethods.SaveProgrammedAbsence(ProgrammedAbsence, oState, bolAudit)
    End Function

    Public Function DeleteProgrammedAbsence(ByVal ProgrammedAbsence As roProgrammedAbsence, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IProgrammedAbsencesSvc.DeleteProgrammedAbsence
        Return ProgrammedAbsencesMethods.DeleteProgrammedAbsence(ProgrammedAbsence, oState, bolAudit)
    End Function

    Public Function GetProgrammedCauses(ByVal IDEmployee As Integer, ByVal xBegin As Date, ByVal xEnd As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IProgrammedAbsencesSvc.GetProgrammedCauses
        Return ProgrammedAbsencesMethods.GetProgrammedCauses(IDEmployee, xBegin, xEnd, oState)
    End Function
End Class
