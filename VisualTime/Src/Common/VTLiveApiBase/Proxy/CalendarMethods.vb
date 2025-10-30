Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTCalendar

Public Class CalendarMethods

    Public Shared Function GetCalenar(xFirstDay As Date, xLastDay As Date, strEmployeeFilter As String, typeView As CalendarView, detailLevel As CalendarDetailLevel, bLoadChilds As Boolean, ByVal oState As roWsState, strAssignmentFilter As String, bolShiftData As Boolean, bolShowIndictments As Boolean, bolShowPunches As Boolean, bLoadSeatingCapacity As Boolean) As roCalendarResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCalendarManager As New roCalendarManager(bState)
        Dim oCalendar As roCalendar = oCalendarManager.Load(xFirstDay, xLastDay, strEmployeeFilter, typeView, detailLevel, bLoadChilds, True, strAssignmentFilter, bolShiftData, bolShowIndictments, bolShowPunches, bLoadSeatingCapacity)

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCalendarManager.State, newGState)

        Dim genericResponse As New roCalendarResponse
        genericResponse.Calendar = oCalendar
        genericResponse.oState = newGState
        genericResponse.CalendarResult.CalendarDataResult = {}
        genericResponse.CalendarResult.Status = CalendarStatusEnum.OK

        Return genericResponse
    End Function

    Public Shared Function AddIndictmentsToCalendar(oCalendar As roCalendar, ByVal oState As roWsState) As roCalendarIndictmentResultResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCalendarManager As New roCalendarManager(bState)
        Dim oCalendarIndictmentResult As New roCalendarIndictmentResult

        oCalendarIndictmentResult.Status = CalendarStatusEnum.KO
        Dim bolret As Boolean = oCalendarManager.AddIndictmentsToCalendar(oCalendar)
        If bolret Then
            oCalendarIndictmentResult.Status = CalendarStatusEnum.OK
            oCalendarIndictmentResult.Calendar = oCalendar
        End If

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCalendarManager.State, newGState)

        Dim genericResponse As New roCalendarIndictmentResultResponse
        genericResponse.CalendarIndictment = oCalendarIndictmentResult
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetCalendarCoverage(xFirstDay As Date, xLastDay As Date, strEmployeeFilter As String, ByVal oState As roWsState, strAssignmentFilter As String) As roCalendarCoverageDaysResponse

        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarPeriodCoverageState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oRet As roCalendarCoverageDay() = Nothing

        If strEmployeeFilter.Count(Function(c As Char) c = "A") = 1 And strEmployeeFilter.Count(Function(c As Char) c = "B") = 0 Then
            oRet = roCalendarPeriodCoverageManager.LoadCoverageByCalendar(xFirstDay, xLastDay, strEmployeeFilter, bState, strAssignmentFilter)
        End If

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)

        Dim genericResponse As New roCalendarCoverageDaysResponse
        genericResponse.CalendarCoverageDays = oRet
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function SaveCalenar(oCalendar As roCalendar, ByVal oState As roWsState) As roCalendarResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCalendarManager As New roCalendarManager(bState)
        Dim oCalendarRes As roCalendarResult = oCalendarManager.Save(oCalendar, True)

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCalendarManager.State, newGState)

        Dim genericResponse As New roCalendarResponse
        genericResponse.Calendar = Nothing
        genericResponse.oState = newGState
        genericResponse.CalendarResult = oCalendarRes

        Return genericResponse
    End Function

    Public Shared Function GetCalendarConfig(ByVal oState As roWsState) As roCalendarPassportConfigResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCalendarManager As New roCalendarManager(bState)
        Dim oCalendarRes As roCalendarPassportConfig = oCalendarManager.CalendarConfig

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCalendarManager.State, newGState)

        Dim genericResponse As New roCalendarPassportConfigResponse
        genericResponse.CalendarPassportConfig = oCalendarRes
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function SaveCalendarConfig(oConfig As roCalendarPassportConfig, ByVal oState As roWsState) As roStandarResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCalendarManager As New roCalendarManager(bState)
        Dim bRes As Boolean = oCalendarManager.SaveCalendarPassportConfig(oConfig)

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCalendarManager.State, newGState)

        Dim genericResponse As New roStandarResponse
        genericResponse.Result = bRes
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetCalendarDayData(IDEmployee As Integer, IDGroup As Integer, xDate As Date, iTypeView As CalendarView, detailLevel As CalendarDetailLevel, bLoadPunches As Boolean, ByVal oState As roWsState, Optional bLoadSeatingCapacity As Boolean = False) As roCalendarRowDayDataResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCalendarManager As New roCalendarManager(bState)
        Dim bRet As roCalendarRowDayData = Nothing

        Dim oCalendar = oCalendarManager.Load(xDate, xDate, "B" & IDEmployee, iTypeView, detailLevel, False, False, "", False, False, bLoadPunches, bLoadSeatingCapacity)

        If oCalendar IsNot Nothing AndAlso oCalendar.CalendarData IsNot Nothing AndAlso oCalendar.CalendarData(0).PeriodData IsNot Nothing AndAlso oCalendar.CalendarData(0).PeriodData.DayData.Count > 0 Then
            bRet = oCalendar.CalendarData(0).PeriodData.DayData(0)
        End If

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCalendarManager.State, newGState)

        Dim genericResponse As New roCalendarRowDayDataResponse
        genericResponse.CalendarDay = bRet
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetCalendarDayHourData(IDEmployee As Integer, IDGroup As Integer, xDate As Date, IDShift As Integer, StartFloating As Date, ByVal detailLevel As CalendarDetailLevel, oCalendarRowShiftData As roCalendarRowShiftData, ByVal oState As roWsState) As roCalendarRowHourDataResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarRowHourDataState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCalendarManager As New roCalendarRowHourDataManager(bState)
        Dim dayHourData As roCalendarRowHourData() = oCalendarManager.GetCalendarDayHourData(IDEmployee, IDGroup, xDate, IDShift, StartFloating, detailLevel, oCalendarRowShiftData)

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)

        Dim genericResponse As New roCalendarRowHourDataResponse
        genericResponse.CalendarRowHourData = dayHourData
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetCalendarDayCoverageData(IDGroup As Integer, xDate As Date, ByVal oState As roWsState, xAssignments As String) As roCalendarCoverageDayResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCalendarManager As New roCalendarManager(bState)

        Dim oRet As roCalendarCoverageDay = Nothing

        Dim strEmployeeFilter = "A" & IDGroup.ToString

        Dim oCalendarPeriodCoverageState As New roCalendarPeriodCoverageState(oState.IDPassport)
        Dim xoCalendarCoverageDay As roCalendarCoverageDay() = roCalendarPeriodCoverageManager.LoadCoverageByCalendar(xDate, xDate, strEmployeeFilter, oCalendarPeriodCoverageState, xAssignments)

        If Not xoCalendarCoverageDay Is Nothing AndAlso xoCalendarCoverageDay.Count > 0 Then
            oRet = xoCalendarCoverageDay(0)
        End If

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCalendarManager.State, newGState)

        Dim genericResponse As New roCalendarCoverageDayResponse
        genericResponse.CalendarCoverageDay = oRet
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function ExportCalendarToExcel(oCalendar As roCalendar, ByVal oState As roWsState) As roCalendarFile
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCalendarManager As New roCalendarManager(bState)

        Dim oRet As Byte() = oCalendarManager.ExportToExcel(oCalendar)

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCalendarManager.State, newGState)

        Dim genericResponse As New roCalendarFile
        genericResponse.XlsByteArray = oRet
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetCalendarFromExcel(arrFile() As Byte, strEmployees As String, dStartDate As Date, dEndDate As Date, bolCopyMainShifts As Boolean, bolCopyHolidays As Boolean, bolCopyAlternatives As Boolean, bolKeepHolidays As Boolean, bolKeepLockedDay As Boolean, ByVal oState As roWsState, lstAssignments As String, bLoadChilds As Boolean, bExcelIsTemplate As Boolean) As roExcelCalendar
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCalendarManager As New roCalendarManager(bState)

        Dim strFileNameError As String = String.Empty
        Dim oCalResult As New roCalendarResult
        Dim oCalendar As roCalendar = oCalendarManager.ImportFromExcelToScreen(arrFile, strEmployees, dStartDate, dEndDate, bolCopyMainShifts, bolCopyHolidays, bolCopyAlternatives, bolKeepHolidays, bolKeepLockedDay, strFileNameError, oCalResult, lstAssignments, bLoadChilds,, bExcelIsTemplate)

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCalendarManager.State, newGState)

        Dim genericResponse As New roExcelCalendar
        genericResponse.Calendar = oCalendar
        genericResponse.CalendarResult = oCalResult
        genericResponse.FileNameError = strFileNameError
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function GetShiftDefinition(IDShift As Integer, ByVal oState As roWsState) As roCalendarShiftResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oCalendarManager As New roCalendarShiftManager(bState)
        Dim bRet As roCalendarShift = oCalendarManager.GetShiftDefinition(IDShift)

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)

        Dim genericResponse As New roCalendarShiftResponse
        genericResponse.CalendarShift = bRet
        genericResponse.oState = newGState

        Return genericResponse
    End Function

    Public Shared Function HasAlternativeShifts(ByVal oState As roWsState) As roStandarResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim strErrorTag As String = String.Empty
        Dim bRet As Boolean = roCalendarManager.HasAlternativeShifts()

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        newGState.ErrorText = strErrorTag

        Return New roStandarResponse With {
                .Result = bRet,
                .oState = newGState
            }
    End Function

    Public Shared Function DeleteAlternativeShifts(ByVal oState As roWsState) As roStandarResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCalendarState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim strErrorTag As String = String.Empty
        Dim bRet As Boolean = roCalendarManager.DeleteAlternativeShifts()

        'crear el response genérico
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        newGState.ErrorText = strErrorTag

        Return New roStandarResponse With {
                .Result = bRet,
                .oState = newGState
            }
    End Function

End Class