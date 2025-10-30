Imports Robotics.Base.VTHolidays
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.DTOs

Public Class ProgrammedHolidaysProxy
    Implements IProgrammedHolidaysSvc

    Public Function KeepAlive() As Boolean Implements IProgrammedHolidaysSvc.KeepAlive
        Return True
    End Function

    Function GetProgrammedHolidayById(ByVal idProgrammedHoliday As Long, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProgrammedHolidayResponse Implements IProgrammedHolidaysSvc.GetProgrammedHolidayById
        Return ProgrammedHolidaysMethods.GetProgrammedHolidayById(idProgrammedHoliday, bAudit, oState)
    End Function

    Public Function GetProgrammedHolidays(ByVal idEmployee As Integer, ByVal strWhere As String, ByVal oState As roWsState) As roProgrammedHolidayListResponse Implements IProgrammedHolidaysSvc.GetProgrammedHolidays
        Return ProgrammedHolidaysMethods.GetProgrammedHolidays(idEmployee, strWhere, oState)
    End Function

    Public Function SaveProgrammedHoliday(ByVal oProgrammedHoliday As roProgrammedHoliday, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProgrammedHolidayResponse Implements IProgrammedHolidaysSvc.SaveProgrammedHoliday
        Return ProgrammedHolidaysMethods.SaveProgrammedHoliday(oProgrammedHoliday, bAudit, oState)
    End Function

    Function DeleteProgrammedHoliday(ByVal oProgrammedHoliday As roProgrammedHoliday, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProgrammedHolidayStandarResponse Implements IProgrammedHolidaysSvc.DeleteProgrammedHoliday
        Return ProgrammedHolidaysMethods.DeleteProgrammedHoliday(oProgrammedHoliday, bAudit, oState)
    End Function


End Class
