Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Incidence

Public Class ProgrammedCausesProxy
    Implements IProgrammedCausesSvc

    Public Function KeepAlive() As Boolean Implements IProgrammedCausesSvc.KeepAlive
        Return True
    End Function

    Public Function GetProgrammedCauses(ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IProgrammedCausesSvc.GetProgrammedCauses
        Return ProgrammedCausesMethods.GetProgrammedCauses(IDEmployee, oState)
    End Function
    Public Function GetProgrammedCause(ByVal IDEmployee As Integer, ByVal ProgrammedDate As Date, ByVal IDAbsence As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roProgrammedCause) Implements IProgrammedCausesSvc.GetProgrammedCause
        Return ProgrammedCausesMethods.GetProgrammedCause(IDEmployee, ProgrammedDate, IDAbsence, oState, bolAudit)
    End Function
    Public Function ValidateProgrammedCause(ByVal ProgrammedCause As roProgrammedCause, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IProgrammedCausesSvc.ValidateProgrammedCause
        Return ProgrammedCausesMethods.ValidateProgrammedCause(ProgrammedCause, oState)
    End Function
    Public Function SaveProgrammedCause(ByVal ProgrammedCause As roProgrammedCause, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roProgrammedCause) Implements IProgrammedCausesSvc.SaveProgrammedCause
        Return ProgrammedCausesMethods.SaveProgrammedCause(ProgrammedCause, oState, bolAudit)
    End Function
    Public Function DeleteProgrammedCause(ByVal ProgrammedCause As roProgrammedCause, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IProgrammedCausesSvc.DeleteProgrammedCause
        Return ProgrammedCausesMethods.DeleteProgrammedCause(ProgrammedCause, oState, bolAudit)
    End Function

End Class
