Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase.Extensions.VTLiveTasks

Public Class LiveTasksProxy
    Implements ILiveTasksSvc

    Public Function KeepAlive() As Boolean Implements ILiveTasksSvc.KeepAlive
        Return True
    End Function

    Public Function GetLiveTaskStatus(ByVal IDLiveTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roLiveTask) Implements ILiveTasksSvc.GetLiveTaskStatus

        Return LiveTasksMethods.GetLiveTaskStatus(IDLiveTask, oState)
    End Function
    Public Function RemoveCompletedTask(IDLiveTask As Integer, oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ILiveTasksSvc.RemoveCompletedTask
        Return LiveTasksMethods.RemoveCompletedTask(IDLiveTask, oState)
    End Function


    Public Function MassMarkConsents(ByVal strDestinationEmployeesFilter As String, ByVal eConsentType As ConsentTypeEnum, ByVal bMarkAsDelivered As Boolean, ByVal strConsentText As String,
                                           ByVal xActionDate As DateTime, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.MassMarkConsents

        Return LiveTasksMethods.MassMarkConsents(strDestinationEmployeesFilter, eConsentType, bMarkAsDelivered, strConsentText, xActionDate, oState, bAudit)
    End Function


    Public Function ExecuteAIPlanner(ByVal bUpdateSchedule As Boolean, ByVal iIdPassport As Integer, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime,
                                       ByVal idOrgChartNode As Integer, ByVal pUnitFilter As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.ExecuteAIPlanner

        Return LiveTasksMethods.ExecuteAIPlanner(bUpdateSchedule, iIdPassport, xBeginDate, xEndDate, idOrgChartNode, pUnitFilter, oState, bAudit)
    End Function


    Public Function CreateAnalyticTask(ByVal analyticType As roLiveAnalyticType, ByVal idView As Integer, ByVal idLayout As Integer, ByVal iIdPassport As Integer, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime,
                                       ByVal strEmployees As String, ByVal strConcepts As String, ByVal strUserFields As String, ByVal strCostCenters As String, ByVal bIncludeZeroValues As Boolean,
                                       ByVal bIncludeUndefinedCostCenters As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean, ByVal strRequestTypes As String) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.CreateAnalyticTask

        Return LiveTasksMethods.CreateAnalyticTask(analyticType, idView, idLayout, iIdPassport, xBeginDate, xEndDate, strEmployees, strConcepts, strUserFields, strCostCenters, bIncludeZeroValues, bIncludeUndefinedCostCenters, oState, bAudit, strRequestTypes)
    End Function

    ''' <summary>
    ''' Crea primero una <see cref="ReportExecution"/> en la tabla ReportExecutions
    ''' y después crea una <see cref="roLiveTask"/>
    ''' </summary>
    ''' <param name="reportId">El ID del <see cref="Report"/> que se va a generar</param>
    ''' <param name="executionTime">La fecha en que se va a ejecutar el report.</param>
    ''' <param name="IdPassport">El ID del passport de quien lo ha ejecutado</param>
    ''' <param name="oState">El estado.</param>
    ''' <param name="bAudit">Parámetro que indica si la acción se audita o no.</param>
    ''' <returns>Retorna un <see cref="Integer"/> equivalente a la ID de la <see cref="roLiveTask"/> generada.</returns>
    Public Function CreateReportTaskDX(ByVal isPlannedTask As Boolean, ByVal reportId As Integer, ByVal reportParameters As String, ByVal IdPassport As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.CreateReportTaskDX

        Return LiveTasksMethods.CreateReportTaskDX(isPlannedTask, reportId, reportParameters, IdPassport, oState, bAudit)
    End Function

    Public Function GetReportsTaksStatus(ByVal oTaskStatus As roLiveTaskStatus, ByVal onlyUser As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ILiveTasksSvc.GetReportsTaksStatus

        Return LiveTasksMethods.GetReportsTaksStatus(oTaskStatus, onlyUser, oState)
    End Function



    Public Function CompleteTasksAndProjectsBackground(ByVal strTaskIDs As String, ByVal strProjectNames As String,
                                           ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.CompleteTasksAndProjectsBackground

        Return LiveTasksMethods.CompleteTasksAndProjectsBackground(strTaskIDs, strProjectNames, oState, bAudit)
    End Function


    Public Function AssignCentersInBackground(ByVal tbIncidences As DataTable, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date,
                                           ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.AssignCentersInBackground

        Return LiveTasksMethods.AssignCentersInBackground(tbIncidences, xBeginPeriod, xEndPeriod, oState, bAudit)
    End Function


    Public Function JustifiedIncidencesInBackground(ByVal tbIncidences As DataTable, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date,
                                           ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.JustifiedIncidencesInBackground

        Return LiveTasksMethods.JustifiedIncidencesInBackground(tbIncidences, xBeginPeriod, xEndPeriod, oState, bAudit)
    End Function


    Public Function CreateTaskWithoutParameters(ByVal oTaskType As roLiveTaskTypes, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.CreateTaskWithoutParameters

        Return LiveTasksMethods.CreateTaskWithoutParameters(oTaskType, oState, bAudit)
    End Function


    Public Function DeleteLiveTask(ByVal IDTask As Integer, ByVal Action As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ILiveTasksSvc.DeleteLiveTask

        Return LiveTasksMethods.DeleteLiveTask(IDTask, Action, oState)
    End Function


    Public Function MassProgrammedAbsenceBackground(ByVal strDestinationEmployees As String, ByVal strFilters As String, ByVal strUserFieldsFilter As String, ByVal IDCause As Integer,
                                           ByVal xBeginDateTime As Date, ByVal xEndDateTime As Date, ByVal strDescription As String, ByVal MinDuration As Double, ByVal MaxDuration As Double,
                                           ByVal MaxDays As Integer, ByVal BeginHour As DateTime, ByVal EndHour As DateTime, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.MassProgrammedAbsenceBackground

        Return LiveTasksMethods.MassProgrammedAbsenceBackground(strDestinationEmployees, strFilters, strUserFieldsFilter, IDCause, xBeginDateTime, xEndDateTime, strDescription, MinDuration, MaxDuration, MaxDays, BeginHour, EndHour, oState, bAudit)
    End Function


    Public Function MassPunchBackground(ByVal strDestinationEmployees As String, ByVal strFilters As String, ByVal strUserFieldsFilter As String, ByVal IDTerminal As Integer, ByVal IDCause As Integer,
                                           ByVal xDateTime As Date, ByVal strDirection As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.MassPunchBackground

        Return LiveTasksMethods.MassPunchBackground(strDestinationEmployees, strFilters, strUserFieldsFilter, IDTerminal, IDCause, xDateTime, strDirection, oState, bAudit)
    End Function


    Public Function AssignCauseInBackground(ByVal strDestinationEmployeesFilter As String, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date, ByVal intIDCause As Integer,
                                           ByVal bolCompletedDay As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.AssignCauseInBackground

        Return LiveTasksMethods.AssignCauseInBackground(strDestinationEmployeesFilter, xBeginPeriod, xEndPeriod, intIDCause, bolCompletedDay, oState, bAudit)
    End Function


    Public Function CreateMessageForEmployees(ByVal strDestinationEmployeesFilter As String, ByVal strMessage As String, ByVal strScheduleRule As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.CreateMessageForEmployees

        Return LiveTasksMethods.CreateMessageForEmployees(strDestinationEmployeesFilter, strMessage, strScheduleRule, oState, bAudit)
    End Function


    Public Function CreateSetLockDateForEmployees(ByVal strDestinationEmployeesFilter As String, ByVal xLockDate As DateTime, ByVal bApplyGlobalLockDate As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.CreateSetLockDateForEmployees

        Return LiveTasksMethods.CreateSetLockDateForEmployees(strDestinationEmployeesFilter, xLockDate, bApplyGlobalLockDate, oState, bAudit)
    End Function


    Public Function CopyEmployeesBackground(ByVal intSourceIDEmployee As Integer, ByVal strDestinationEmployeesFilter As String, ByVal ckCopyUserFields As Boolean,
                                     ByVal userFieldHistory As ArrayList, ByVal userFieldNoHistory As ArrayList, ByVal ckLabAgree As Boolean, ByVal ckCopyAssignments As Boolean, ByVal oAssignments As ArrayList, ByVal copySchedule As Boolean,
                                     ByVal xStart As Date, ByVal xEnd As Date, ByVal bolCopyMainShifts As Boolean, ByVal bolCopyAlternativeShifts As Boolean, ByVal bolCopyHolidays As Boolean,
                                     ByVal bolKeepBloquedDays As Boolean, ByVal bolKeepHolidays As Boolean, ByVal ckCopyCenters As Boolean, ByVal oCenters As ArrayList, ByVal bAudit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.CopyEmployeesBackground

        Return LiveTasksMethods.CopyEmployeesBackground(intSourceIDEmployee, strDestinationEmployeesFilter, ckCopyUserFields, userFieldHistory, userFieldNoHistory, ckLabAgree, ckCopyAssignments, oAssignments, copySchedule,
                                     xStart, xEnd, bolCopyMainShifts, bolCopyAlternativeShifts, bolCopyHolidays, bolKeepBloquedDays, bolKeepHolidays, ckCopyCenters, oCenters, bAudit, oState)

    End Function


    Public Function CopyShiftsInBackground(ByVal intSourceIDEmployee As Integer, ByVal strDestinationEmployeesFilter As String, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date,
                                            ByVal bolCopyMainShifts As Boolean, ByVal bolCopyAlternativeShifts As Boolean, ByVal bolCopyHolidays As Boolean,
                                            ByVal bolKeepBloquedDays As Boolean, ByVal bolKeepHolidays As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.CopyShiftsInBackground

        Return LiveTasksMethods.CopyShiftsInBackground(intSourceIDEmployee, strDestinationEmployeesFilter, xBeginPeriod, xEndPeriod,
                                            bolCopyMainShifts, bolCopyAlternativeShifts, bolCopyHolidays, bolKeepBloquedDays, bolKeepHolidays, oState, bAudit)
    End Function


    Public Function CopyAdvShiftsEmployeesInBackground(ByVal intSourceIDEmployee As ArrayList, ByVal lstDestinationEmployees As ArrayList, ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal strShiftsSelected As String,
                                            ByVal bolCopyMainShifts As Boolean, ByVal bolCopyAlternativeShifts As Boolean, ByVal bolCopyHolidays As Boolean,
                                            ByVal bolKeepBloquedDays As Boolean, ByVal bolKeepHolidays As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.CopyAdvShiftsEmployeesInBackground

        Return LiveTasksMethods.CopyAdvShiftsEmployeesInBackground(intSourceIDEmployee, lstDestinationEmployees, xBeginDate, xEndDate, strShiftsSelected,
                                            bolCopyMainShifts, bolCopyAlternativeShifts, bolCopyHolidays, bolKeepBloquedDays, bolKeepHolidays, oState, bAudit)
    End Function


    Public Function AssignWeekShiftsInBackground(ByVal strDestinationEmployeesFilter As String, ByVal lstWeekShifts As ArrayList, ByVal lstWeekStartShifts As Generic.List(Of DateTime),
                                     ByVal lstWeekAssignments As Generic.List(Of Integer), ByVal xBeginDate As Date, ByVal xEndDate As Date,
                                     ByVal bolKeepBloquedDays As Boolean, ByVal bolKeepHolidaysDays As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.AssignWeekShiftsInBackground
        Return LiveTasksMethods.AssignWeekShiftsInBackground(strDestinationEmployeesFilter, lstWeekShifts, lstWeekStartShifts,
                                     lstWeekAssignments, xBeginDate, xEndDate, bolKeepBloquedDays, bolKeepHolidaysDays, oState)
    End Function


    Public Function AssignScheduleTemplateInBackground(ByVal _IDTemplate As Integer, ByVal _IDGroup As Integer, ByVal _UserFieldConditions As Generic.List(Of roUserFieldCondition), ByVal _Year As Integer, ByVal _IDShift As Integer, ByVal _StartShift As DateTime, ByVal _LockDays As Boolean, ByVal _IncludeChildGroups As Boolean, ByVal _KeepBloquedDays As Boolean, ByVal _KeepHolidays As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.AssignScheduleTemplateInBackground

        Return LiveTasksMethods.AssignScheduleTemplateInBackground(_IDTemplate, _IDGroup, _UserFieldConditions,
                                     _Year, _IDShift, _StartShift, _LockDays, _IncludeChildGroups, _KeepBloquedDays, _KeepHolidays, oState)
    End Function




    Public Function AssignScheduleTemplatev2InBackground(ByVal _IDTemplate As Integer, ByVal iIDEmployeeDestination As ArrayList, ByVal _IDShift As Integer, ByVal _StartShift As DateTime, ByVal _LockDays As Boolean, ByVal _KeepBloquedDays As Boolean, ByVal _KeepHolidays As Boolean, ByVal oState As roWsState, ByVal _FeastDays As Boolean, ByVal _EmployeeFilter As String, ByVal _IdUserField As String, ByVal _TxtUserField As String) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.AssignScheduleTemplatev2InBackground

        Return LiveTasksMethods.AssignScheduleTemplatev2InBackground(_IDTemplate, iIDEmployeeDestination, _IDShift, _StartShift,
                                     _LockDays, _KeepBloquedDays, _KeepHolidays, oState, _FeastDays, _EmployeeFilter, _IdUserField, _TxtUserField)
    End Function



    Public Function CreateCalendarSpecialPaste(ByVal _copyStartDate As DateTime, ByVal _copyEndDate As DateTime, ByVal _DestStartDate As DateTime, ByVal iIDEmployeeSource As Integer, ByVal iIDEmployeeDestination As Generic.List(Of Integer),
                                               ByVal iRepeatMode As Integer, ByVal strRepeatModeValue As String, ByVal iRepeatStartMode As Integer, ByVal strRepeatStartModeValue As String,
                                               ByVal iRepeatSkipMode As Integer, ByVal iRepeatSkiptimes As Integer, ByVal strRepeatSkipModeValue As String, ByVal iBloquedMode As Integer, ByVal iHolidaysMode As Integer,
                                               ByVal iCopyMainShifts As Integer, ByVal iCopyHolidays As Integer, ByVal iLockDestDays As Integer, ByVal iCopyAssignment As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.CreateCalendarSpecialPaste

        Return LiveTasksMethods.CreateCalendarSpecialPaste(_copyStartDate, _copyEndDate, _DestStartDate, iIDEmployeeSource, iIDEmployeeDestination, iRepeatMode, strRepeatModeValue, iRepeatStartMode, strRepeatStartModeValue,
                                               iRepeatSkipMode, iRepeatSkiptimes, strRepeatSkipModeValue, iBloquedMode, iHolidaysMode, iCopyMainShifts, iCopyHolidays, iLockDestDays, iCopyAssignment, oState)
    End Function


    Public Function CreateBudgetSpecialPaste(ByVal _copyStartDate As DateTime, ByVal _copyEndDate As DateTime, ByVal _DestStartDate As DateTime, ByVal iIDNodeSource As Integer, ByVal iIDProductiveUnitSource As Integer,
                                               ByVal iRepeatMode As Integer, ByVal iHolidaysMode As Integer, ByVal strRepeatModeValue As String, ByVal bolCopyEmployees As Boolean,
                                                ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.CreateBudgetSpecialPaste
        Return LiveTasksMethods.CreateBudgetSpecialPaste(_copyStartDate, _copyEndDate, _DestStartDate, iIDNodeSource, iIDProductiveUnitSource,
                                               iRepeatMode, iHolidaysMode, strRepeatModeValue, bolCopyEmployees, oState)
    End Function


    Public Function CreateImportBackground(ByVal IDImport As Integer, ByVal oFileOrig() As Byte, ByVal oFileSchema() As Byte, ByVal EmployeesSelected As String, ByVal ScheduleBeginDate As Date, ByVal ScheduleEndDate As Date, ByVal oState As roWsState,
                                           ByVal ExcelIsTemplate As Integer, ByVal CopyMainShifts As Boolean, ByVal CopyHolidays As Boolean, ByVal KeepHolidays As Boolean, ByVal KeepLockedDays As Boolean) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.CreateImportBackground

        Return LiveTasksMethods.CreateImportBackground(IDImport, oFileOrig, oFileSchema, EmployeesSelected, ScheduleBeginDate, ScheduleEndDate, oState,
                                           ExcelIsTemplate, CopyMainShifts, CopyHolidays, KeepHolidays, KeepLockedDays)
    End Function


    Public Function CreateExportBackground(ByVal IDExport As Integer, ByVal FileType As String, ByVal EmployeesSelected As String, ByVal inExcel2007Format As Boolean, ByVal ScheduleBeginDate As Date, ByVal ScheduleEndDate As Date,
                                           ByVal isExcelInstalled As Boolean, ByVal ExcelTemplateFile As String, ByVal ConceptGroup As String, ByVal Separator As String, ByVal ProfileType As Integer, ByVal bApplyLockDate As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements ILiveTasksSvc.CreateExportBackground


        Return LiveTasksMethods.CreateExportBackground(IDExport, FileType, EmployeesSelected, inExcel2007Format, ScheduleBeginDate, ScheduleEndDate,
                                           isExcelInstalled, ExcelTemplateFile, ConceptGroup, Separator, ProfileType, bApplyLockDate, oState)
    End Function


    Public Function DownloadFile(ByVal fileName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements ILiveTasksSvc.DownloadFile
        Return LiveTasksMethods.DownloadFile(fileName, oState)
    End Function


    Public Function DownloadFileAzure(ByVal fileName As String, ByVal Container As roLiveQueueTypes, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements ILiveTasksSvc.DownloadFileAzure
        Return LiveTasksMethods.DownloadFileAzure(fileName, Container, oState)
    End Function

    Public Function DownloadFileZippedFromAzure(ByVal fileName As String, ByVal Container As roLiveQueueTypes, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements ILiveTasksSvc.DownloadFileZippedFromAzure
        Return LiveTasksMethods.DownloadFileZippedFromAzure(fileName, Container, oState)
    End Function


    Public Function DownloadFileZipped(ByVal fileName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements ILiveTasksSvc.DownloadFileZipped
        Return LiveTasksMethods.DownloadFileZipped(fileName, oState)
    End Function

    Public Function UploadFile(ByVal fileName As String, ByVal fileConent As Byte(), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ILiveTasksSvc.UploadFile
        Return LiveTasksMethods.UploadFile(fileName, fileConent, oState)
    End Function


    Public Function DuplicateFile(ByVal fileName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ILiveTasksSvc.DuplicateFile
        Return LiveTasksMethods.DuplicateFile(fileName, oState)
    End Function


    Public Function RemoveFile(ByVal fileName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ILiveTasksSvc.RemoveFile
        Return LiveTasksMethods.RemoveFile(fileName, oState)
    End Function

    Public Function FileExists(ByVal fileName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ILiveTasksSvc.FileExists
        Return LiveTasksMethods.FileExists(fileName, oState)
    End Function


End Class
