Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTHolidays
Imports Robotics.VTBase

Public Class CalendarProxy
    Implements ICalendarSvc

    Public Function KeepAlive() As Boolean Implements ICalendarSvc.KeepAlive
        Return True
    End Function

    Public Function GetCalenar(xFirstDay As Date, xLastDay As Date, strEmployeeFilter As String, typeView As CalendarView, detailLevel As CalendarDetailLevel, bLoadChilds As Boolean, ByVal oState As roWsState, strAssignmentFilter As String, bolShiftData As Boolean, bolShowIndictments As Boolean, bolShowPunches As Boolean) As roCalendarResponse Implements ICalendarSvc.GetCalenar
        Return CalendarMethods.GetCalenar(xFirstDay, xLastDay, strEmployeeFilter, typeView, detailLevel, bLoadChilds, oState, strAssignmentFilter, bolShiftData, bolShowIndictments, bolShowPunches)
    End Function

    Public Function AddIndictmentsToCalendar(oCalendar As roCalendar, ByVal oState As roWsState) As roCalendarIndictmentResultResponse Implements ICalendarSvc.AddIndictmentsToCalendar
        Return CalendarMethods.AddIndictmentsToCalendar(oCalendar, oState)
    End Function

    Public Function GetCalendarCoverage(xFirstDay As Date, xLastDay As Date, strEmployeeFilter As String, ByVal oState As roWsState, strAssignmentFilter As String) As roCalendarCoverageDaysResponse Implements ICalendarSvc.GetCalendarCoverage
        Return CalendarMethods.GetCalendarCoverage(xFirstDay, xLastDay, strEmployeeFilter, oState, strAssignmentFilter)
    End Function

    Public Function SaveCalenar(oCalendar As roCalendar, ByVal oState As roWsState) As roCalendarResponse Implements ICalendarSvc.SaveCalenar
        Return CalendarMethods.SaveCalenar(oCalendar, oState)
    End Function

    Public Function GetCalendarConfig(ByVal oState As roWsState) As roCalendarPassportConfigResponse Implements ICalendarSvc.GetCalendarConfig
        Return CalendarMethods.GetCalendarConfig(oState)
    End Function

    Public Function SaveCalendarConfig(oConfig As roCalendarPassportConfig, ByVal oState As roWsState) As roStandarResponse Implements ICalendarSvc.SaveCalendarConfig
        Return CalendarMethods.SaveCalendarConfig(oConfig, oState)
    End Function

    Public Function GetCalendarDayData(IDEmployee As Integer, IDGroup As Integer, xDate As Date, iTypeView As CalendarView, detailLevel As CalendarDetailLevel, bLoadPunches As Boolean, ByVal oState As roWsState) As roCalendarRowDayDataResponse Implements ICalendarSvc.GetCalendarDayData
        Return CalendarMethods.GetCalendarDayData(IDEmployee, IDGroup, xDate, iTypeView, detailLevel, bLoadPunches, oState)
    End Function

    Public Function GetCalendarDayHourData(IDEmployee As Integer, IDGroup As Integer, xDate As Date, IDShift As Integer, StartFloating As Date, ByVal detailLevel As CalendarDetailLevel, oCalendarRowShiftData As roCalendarRowShiftData, ByVal oState As roWsState) As roCalendarRowHourDataResponse Implements ICalendarSvc.GetCalendarDayHourData
        Return CalendarMethods.GetCalendarDayHourData(IDEmployee, IDGroup, xDate, IDShift, StartFloating, detailLevel, oCalendarRowShiftData, oState)
    End Function

    Public Function GetCalendarDayCoverageData(IDGroup As Integer, xDate As Date, ByVal oState As roWsState, xAssignments As String) As roCalendarCoverageDayResponse Implements ICalendarSvc.GetCalendarDayCoverageData
        Return CalendarMethods.GetCalendarDayCoverageData(IDGroup, xDate, oState, xAssignments)
    End Function

    Public Function ExportCalendarToExcel(oCalendar As roCalendar, ByVal oState As roWsState) As roCalendarFile Implements ICalendarSvc.ExportCalendarToExcel
        Return CalendarMethods.ExportCalendarToExcel(oCalendar, oState)
    End Function

    Public Function GetCalendarFromExcel(arrFile() As Byte, strEmployees As String, dStartDate As Date, dEndDate As Date, bolCopyMainShifts As Boolean, bolCopyHolidays As Boolean, bolCopyAlternatives As Boolean, bolKeepHolidays As Boolean, bolKeepLockedDay As Boolean, ByVal oState As roWsState, lstAssignments As String, bLoadChilds As Boolean, bExcelIsTemplate As Boolean) As roExcelCalendar Implements ICalendarSvc.GetCalendarFromExcel
        Return CalendarMethods.GetCalendarFromExcel(arrFile, strEmployees, dStartDate, dEndDate, bolCopyMainShifts, bolCopyHolidays, bolCopyAlternatives, bolKeepHolidays, bolKeepLockedDay, oState, lstAssignments, bLoadChilds, bExcelIsTemplate)
    End Function

    Public Function ChangeCalenarModeToV1(ByVal oState As roWsState) As roStandarResponse Implements ICalendarSvc.ChangeCalenarModeToV1
        Return CalendarMethods.ChangeCalenarModeToV1(oState)
    End Function

    Public Function ChangeCalenarModeToV2(ByVal oState As roWsState) As roStandarResponse Implements ICalendarSvc.ChangeCalenarModeToV2
        Return CalendarMethods.ChangeCalenarModeToV2(oState)
    End Function

    Public Function GetShiftDefinition(IDShift As Integer, ByVal oState As roWsState) As roCalendarShiftResponse Implements ICalendarSvc.GetShiftDefinition
        Return CalendarMethods.GetShiftDefinition(IDShift, oState)
    End Function

    Public Function HasAlternativeShifts(ByVal oState As roWsState) As roStandarResponse Implements ICalendarSvc.HasAlternativeShifts
        Return CalendarMethods.HasAlternativeShifts(oState)
    End Function

    Public Function DeleteAlternativeShifts(ByVal oState As roWsState) As roStandarResponse Implements ICalendarSvc.DeleteAlternativeShifts
        Return CalendarMethods.DeleteAlternativeShifts(oState)
    End Function
End Class
