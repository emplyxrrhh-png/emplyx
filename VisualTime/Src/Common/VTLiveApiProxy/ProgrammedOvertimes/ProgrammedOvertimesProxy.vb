Imports Robotics.Base.VTHolidays
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.DTOs


Public Class ProgrammedOvertimesProxy
    Implements IProgrammedOvertimesSvc

    Public Function KeepAlive() As Boolean Implements IProgrammedOvertimesSvc.KeepAlive
        Return True
    End Function


    Function GetProgrammedOvertimeById(ByVal idProgrammedOvertime As Long, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProgrammedOvertimeResponse Implements IProgrammedOvertimesSvc.GetProgrammedOvertimeById
        Return ProgrammedOvertimesMethods.GetProgrammedOvertimeById(idProgrammedOvertime, bAudit, oState)
    End Function

    Public Function GetProgrammedOvertimes(ByVal idEmployee As Integer, ByVal strWhere As String, ByVal oState As roWsState) As roProgrammedOvertimeListResponse Implements IProgrammedOvertimesSvc.GetProgrammedOvertimes
        Return ProgrammedOvertimesMethods.GetProgrammedOvertimes(idEmployee, strWhere, oState)
    End Function

    Public Function SaveProgrammedOvertime(ByVal oProgrammedOvertime As roProgrammedOvertime, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProgrammedOvertimeResponse Implements IProgrammedOvertimesSvc.SaveProgrammedOvertime
        Return ProgrammedOvertimesMethods.SaveProgrammedOvertime(oProgrammedOvertime, bAudit, oState)
    End Function

    Function DeleteProgrammedOvertime(ByVal oProgrammedOvertime As roProgrammedOvertime, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProgrammedOvertimeStandarResponse Implements IProgrammedOvertimesSvc.DeleteProgrammedOvertime
        Return ProgrammedOvertimesMethods.DeleteProgrammedOvertime(oProgrammedOvertime, bAudit, oState)
    End Function


End Class
