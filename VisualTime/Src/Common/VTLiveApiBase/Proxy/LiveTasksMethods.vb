Imports System.Data
Imports System.Data.Common
Imports ReportGenerator.Services
Imports Robotics.Azure
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTDataLink
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks

Public Class LiveTasksMethods

    Public Shared Function GetLiveTaskStatus(ByVal IDLiveTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roLiveTask)
        Dim oResult As New roGenericVtResponse(Of roLiveTask)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oTask As New roLiveTask(IDLiveTask, bState)

        If oTask.State.Result = LiveTasksResultEnum.NoError AndAlso oTask.Action <> String.Empty Then
            Try
                Dim oSecurityState As New roSecurityState
                roWsStateManager.CopyTo(oState, oSecurityState)

                SessionHelper.UpdateLastAccessTime(oState.SessionID, oState.IDPassport, oSecurityState)

                If oTask.Status > 1 AndAlso oTask.Action <> roLiveTaskTypes.Export.ToString().ToUpper AndAlso
                    oTask.Action <> roLiveTaskTypes.AnalyticsTask.ToString().ToUpper AndAlso
                    oTask.Action <> roLiveTaskTypes.ReportTaskDX.ToString().ToUpper AndAlso
                    oTask.Action <> roLiveTaskTypes.AIPlannerTask.ToString().ToUpper Then
                    oTask.Delete()
                End If
            Catch ex As Exception
                oTask.Status = 3
                oTask.State.Result = LiveTasksResultEnum.CouldNotQueryStatus
            End Try
        Else
            oTask.Status = 3
            oTask.State.Result = LiveTasksResultEnum.CouldNotQueryStatus
        End If

        oResult.Value = oTask

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oTask.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function RemoveCompletedTask(ByVal IDLiveTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oTask As New roLiveTask(IDLiveTask, bState)

        If oTask IsNot Nothing AndAlso oTask.ID > 0 Then
            oResult.Value = oTask.Delete()
        Else
            oResult.Value = False
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function ExecuteAIPlanner(ByVal bUpdateSchedule As Boolean, ByVal iIdPassport As Integer, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime,
                                       ByVal idOrgChartNode As Integer, ByVal pUnitFilter As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        oParameters.Add("UpdateSchedule", If(bUpdateSchedule, "1", "0"))
        oParameters.Add("IdPassport", iIdPassport)
        oParameters.Add("IdOrgChartNode", idOrgChartNode)
        oParameters.Add("ScheduleBeginDate", xBeginDate.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("ScheduleEndDate", xEndDate.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("UnitFilter", pUnitFilter)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.AIPlannerTask, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function CreateAnalyticTask(ByVal analyticType As roLiveAnalyticType, ByVal idView As Integer, ByVal idLayout As Integer, ByVal iIdPassport As Integer, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime,
                                       ByVal strEmployees As String, ByVal strConcepts As String, ByVal strUserFields As String, ByVal strCostCenters As String, ByVal bIncludeZeroValues As Boolean,
                                       ByVal bIncludeUndefinedCostCenters As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean, ByVal strRequestTypes As String, ByVal strCauses As String) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)

        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        oParameters.Add("AnalyticType", CInt(analyticType))
        oParameters.Add("IdView", idView)
        oParameters.Add("IdLayout", idLayout)
        oParameters.Add("ScheduleBeginDate", xBeginDate.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("ScheduleEndDate", xEndDate.ToString("yyyy/MM/dd HH:mm"))

        oParameters.Add("Employees", strEmployees)
        oParameters.Add("Concepts", strConcepts)
        oParameters.Add("UserFields", strUserFields)
        oParameters.Add("BusinessCenters", strCostCenters)

        oParameters.Add("IncludeZeroValues", If(bIncludeZeroValues, "1", "0"))
        oParameters.Add("IncludeEntriesWithoutBusinessCenter", If(bIncludeUndefinedCostCenters, "1", "0"))
        oParameters.Add("RequestTypes", strRequestTypes)
        oParameters.Add("Incidences", strCauses)
        oParameters.Add("APIVersion", roConstants.DXAnalityc)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.AnalyticsTask, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

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
    Public Shared Function CreateReportTaskDX(ByVal isPlannedTask As Boolean, ByVal reportId As Integer, ByVal reportParameters As String, ByVal viewFields As String, ByVal IdPassport As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)
        Dim oResult As New roGenericVtResponse(Of Integer)

        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim reportExecution As New ReportExecution With {
                .Binary = Nothing,
                .ExecutionDate = DateTime.Now,
                .ReportID = reportId,
                .PassportID = IdPassport,
                .Status = 1
                }

        Dim guidGenerada As Guid = Nothing
        Dim reportExecutionService = New ReportExecutionService()
        guidGenerada = reportExecutionService.InsertExecution(reportExecution)
        Dim oParameters As New roCollection

        oParameters.Add("isPlannedTask", isPlannedTask.ToString)
        oParameters.Add("ReportId", reportId.ToString)
        oParameters.Add("ExecutionTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("Guid", guidGenerada.ToString)
        oParameters.Add("ReportParameters", reportParameters)
        oParameters.Add("ViewFields", viewFields)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.ReportTaskDX, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetReportsTaksStatus(ByVal oTaskStatus As roLiveTaskStatus, ByVal onlyUser As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oRet As DataSet = Nothing
        Dim tb As DataTable = roLiveTask.GetReportsTaksStatus(oTaskStatus, onlyUser, bState)
        If tb IsNot Nothing Then
            If tb.DataSet Is Nothing Then
                oRet = New DataSet()
                oRet.Tables.Add(tb)
            Else
                oRet = tb.DataSet
            End If
        End If

        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function CompleteTasksAndProjectsBackground(ByVal strTaskIDs As String, ByVal strProjectNames As String,
                                           ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        oParameters.Add("Tasks", strTaskIDs)
        oParameters.Add("Projects", strProjectNames)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.CompleteTasksAndProjects, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function AssignCentersInBackground(ByVal tbIncidences As DataTable, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date,
                                           ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        Dim tbChanges As DataTable = tbIncidences.GetChanges()
        Dim lstRows As String = ""
        If Not tbChanges Is Nothing AndAlso tbChanges.Rows.Count > 0 Then
            For Each oRowNew As DataRow In tbChanges.Rows
                If oRowNew.RowState = DataRowState.Added Or oRowNew.RowState = DataRowState.Modified Then
                    Dim strreg As String = ""
                    Dim strAction As String = IIf(oRowNew.RowState = DataRowState.Added, "A", "M")

                    strreg = strAction & "@" & oRowNew.Item("IDEmployee").ToString() & "@" & CDate(oRowNew.Item("Date")).ToString("yyyy/MM/dd HH:mm") & "@" & oRowNew.Item("IDRelatedIncidence").ToString() & "@" & oRowNew.Item("IDCause").ToString() & "@" & CStr(oRowNew.Item("Value")).Replace(roConversions.GetDecimalDigitFormat, ".") & "@" & IIf(oRowNew.Item("DefaultCenter"), "1", "0") & "@" & oRowNew.Item("IDCenter").ToString & "@" & IIf(oRowNew.Item("ManualCenter"), "1", "0") & "@" & oRowNew.Item("IDCenterOld").ToString & "@" & oRowNew.Item("AccrualsRules").ToString() & "@" & oRowNew.Item("DailyRule").ToString() & "@" & oRowNew.Item("AccruedRule").ToString()

                    If lstRows.Length = 0 Then
                        lstRows += strreg
                    Else
                        lstRows += "#" & strreg
                    End If
                End If
            Next
        End If
        oParameters.Add("lstRows", lstRows)
        oParameters.Add("xBeginDate", xBeginPeriod.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("xEndDate", xEndPeriod.ToString("yyyy/MM/dd HH:mm"))

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.AssignCenters, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function SecurityActionsInBackground(ByVal iAction As Integer, strEmployeeFilter As String, strFeature As String, strFilters As String, strUserFieldFilters As String, bLockAccess As Boolean,
                                                       iSourceEmployee As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection
        oParameters.Add("iAction", iAction)
        oParameters.Add("strEmployeeFilter", strEmployeeFilter)
        oParameters.Add("strFilters", strFilters)
        oParameters.Add("strUserFieldFilters", strUserFieldFilters)
        oParameters.Add("strFeature", strFeature)
        oParameters.Add("bLockAccess", If(bLockAccess, 1, 0))
        oParameters.Add("iSourceEmployee", iSourceEmployee)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.EmployeeSecurtiyActions, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function JustifiedIncidencesInBackground(ByVal tbIncidences As DataTable, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date,
                                           ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        Dim tbChanges As DataTable = tbIncidences.GetChanges()
        Dim lstRows As String = ""
        If Not tbChanges Is Nothing AndAlso tbChanges.Rows.Count > 0 Then
            For Each oRowNew As DataRow In tbChanges.Rows
                If oRowNew.RowState = DataRowState.Added Or oRowNew.RowState = DataRowState.Modified Then
                    Dim strreg As String = ""
                    Dim strAction As String = IIf(oRowNew.RowState = DataRowState.Added, "A", "M")

                    strreg = strAction & "@" & oRowNew.Item("IDEmployee").ToString() & "@" & CDate(oRowNew.Item("Date")).ToString("yyyy/MM/dd HH:mm") & "@" & oRowNew.Item("IDRelatedIncidence").ToString() & "@" & oRowNew.Item("IDCause").ToString() & "@" & CStr(oRowNew.Item("Value")).Replace(roConversions.GetDecimalDigitFormat, ".") & "@" & oRowNew.Item("IDCauseOld").ToString & "@" & IIf(roTypes.Any2Boolean(oRowNew.Item("DefaultCenter")), "1", "0") & "@" & oRowNew.Item("IDCenter").ToString & "@" & IIf(Not IsDBNull(oRowNew.Item("ManualCenter")) AndAlso oRowNew.Item("ManualCenter"), "1", "0")

                    If lstRows.Length = 0 Then
                        lstRows += strreg
                    Else
                        lstRows += "#" & strreg
                    End If
                End If
            Next
        End If
        oParameters.Add("lstRows", lstRows)
        oParameters.Add("xBeginDate", xBeginPeriod.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("xEndDate", xEndPeriod.ToString("yyyy/MM/dd HH:mm"))

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.JustifiedIncidences, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function CreateTaskWithoutParameters(ByVal oTaskType As roLiveTaskTypes, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roLiveTask.CreateEmptyLiveTask(oTaskType, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteLiveTask(ByVal IDTask As Integer, ByVal Action As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roLiveTask.DeleteLiveTask(IDTask, Action, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function MassProgrammedAbsenceBackground(ByVal strDestinationEmployees As String, ByVal strFilters As String, ByVal strUserFieldsFilter As String, ByVal IDCause As Integer,
                                           ByVal xBeginDateTime As Date, ByVal xEndDateTime As Date, ByVal strDescription As String, ByVal MinDuration As Double, ByVal MaxDuration As Double,
                                           ByVal MaxDays As Integer, ByVal BeginHour As DateTime, ByVal EndHour As DateTime, massType As eRequestType, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        oParameters.Add("EmployeeGroups", strDestinationEmployees)
        oParameters.Add("EmployeeFilter", strFilters)
        oParameters.Add("EmployeeUserFieldFilter", strUserFieldsFilter)

        oParameters.Add("IDCause", IDCause.ToString)

        oParameters.Add("BeginDateTime", xBeginDateTime.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("EndDateTime", xEndDateTime.ToString("yyyy/MM/dd HH:mm"))

        oParameters.Add("Description", strDescription)

        oParameters.Add("MinDuration", MinDuration.ToString)
        oParameters.Add("MaxDuration", MaxDuration.ToString)

        oParameters.Add("MaxDays", MaxDays.ToString)
        oParameters.Add("BeginHour", BeginHour.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("EndHour", EndHour.ToString("yyyy/MM/dd HH:mm"))

        oParameters.Add("RequestType", massType.ToString())

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.MassProgrammedAbsence, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function MassPunchBackground(ByVal strDestinationEmployees As String, ByVal strFilters As String, ByVal strUserFieldsFilter As String, ByVal IDTerminal As Integer, ByVal IDCause As Integer,
                                           ByVal xDateTime As Date, ByVal strDirection As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        oParameters.Add("EmployeeGroups", strDestinationEmployees)
        oParameters.Add("EmployeeFilter", strFilters)
        oParameters.Add("EmployeeUserFieldFilter", strUserFieldsFilter)

        oParameters.Add("PunchDateTime", xDateTime.ToString("yyyy/MM/dd HH:mm"))

        oParameters.Add("PunchDirection", strDirection)

        oParameters.Add("IDTerminal", IDTerminal.ToString)
        oParameters.Add("IDCause", IDCause.ToString)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.MassPunch, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function AssignCauseInBackground(ByVal strDestinationEmployeesFilter As String, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date, ByVal intIDCause As Integer,
                                           ByVal bolCompletedDay As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        oParameters.Add("lstEmployees", strDestinationEmployeesFilter)
        oParameters.Add("xBeginDate", xBeginPeriod.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("xEndDate", xEndPeriod.ToString("yyyy/MM/dd HH:mm"))

        oParameters.Add("CompletedDay", bolCompletedDay)

        oParameters.Add("IDCause", intIDCause.ToString)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.MassCause, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function CreateMessageForEmployees(ByVal strDestinationEmployeesFilter As String, ByVal strMessage As String, ByVal strScheduleRule As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        oParameters.Add("lstEmployees", strDestinationEmployeesFilter)
        oParameters.Add("schedule", strScheduleRule)
        oParameters.Add("message", strMessage)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.EmployeeMessage, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function CreateSetLockDateForEmployees(ByVal strDestinationEmployeesFilter As String, ByVal xLockDate As DateTime, ByVal bApplyGlobalLockDate As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        oParameters.Add("lstDestinationIDEmployees", strDestinationEmployeesFilter)
        oParameters.Add("xLockDate", xLockDate.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("ckApplyLockDateGlobal", bApplyGlobalLockDate)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.MassLockDate, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function CopyEmployeesBackground(ByVal intSourceIDEmployee As Integer, ByVal strDestinationEmployeesFilter As String, ByVal ckCopyUserFields As Boolean,
                                     ByVal userFieldHistory As List(Of String), ByVal userFieldNoHistory As List(Of String), ByVal ckLabAgree As Boolean, ByVal ckCopyAssignments As Boolean, ByVal oAssignments As List(Of String), ByVal copySchedule As Boolean,
                                     ByVal xStart As Date, ByVal xEnd As Date, ByVal bolCopyMainShifts As Boolean, ByVal bolCopyAlternativeShifts As Boolean, ByVal bolCopyHolidays As Boolean,
                                     ByVal bolKeepBloquedDays As Boolean, ByVal bolKeepHolidays As Boolean, ByVal ckCopyCenters As Boolean, ByVal oCenters As List(Of String), ByVal bAudit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        oParameters.Add("intSourceIDEmployee", intSourceIDEmployee)

        Dim strUserFieldHistory As String = ""
        If userFieldHistory.Count > 0 Then strUserFieldHistory = Strings.Join(userFieldHistory.ToArray(), ",")
        Dim struserFieldNoHistory As String = ""
        If userFieldNoHistory.Count > 0 Then struserFieldNoHistory = Strings.Join(userFieldNoHistory.ToArray(), ",")
        Dim stroAssignments As String = ""
        If oAssignments.Count > 0 Then stroAssignments = Strings.Join(oAssignments.ToArray(), ",")
        Dim stroCenters As String = ""
        If oCenters.Count > 0 Then stroCenters = Strings.Join(oCenters.ToArray(), "@")

        oParameters.Add("lstDestinationIDEmployees", strDestinationEmployeesFilter)
        oParameters.Add("userFieldHistory", strUserFieldHistory)
        oParameters.Add("userFieldNoHistory", struserFieldNoHistory)
        oParameters.Add("oAssignments", stroAssignments)
        oParameters.Add("oCenters", stroCenters)
        oParameters.Add("ckLabAgree", ckLabAgree)
        oParameters.Add("ckCopyUserFields", ckCopyUserFields)
        oParameters.Add("ckCopyAssignments", ckCopyAssignments)
        oParameters.Add("ckCopyCenters", ckCopyCenters)

        oParameters.Add("copySchedule", copySchedule)
        oParameters.Add("xStart", xStart.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("xEnd", xEnd.ToString("yyyy/MM/dd HH:mm"))

        oParameters.Add("CopyMainShifts", bolCopyMainShifts)
        oParameters.Add("CopyAlternativeShifts", bolCopyAlternativeShifts)
        oParameters.Add("CopyHolidays", bolCopyHolidays)

        oParameters.Add("KeepBloquedDays", bolKeepBloquedDays)
        oParameters.Add("KeepHolidaysDays", bolKeepHolidays)
        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.MassCopy, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function CopyShiftsInBackground(ByVal intSourceIDEmployee As Integer, ByVal strDestinationEmployeesFilter As String, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date,
                                            ByVal bolCopyMainShifts As Boolean, ByVal bolCopyAlternativeShifts As Boolean, ByVal bolCopyHolidays As Boolean,
                                            ByVal bolKeepBloquedDays As Boolean, ByVal bolKeepHolidays As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        oParameters.Add("intSourceIDEmployee", intSourceIDEmployee)
        oParameters.Add("lstEmployees", strDestinationEmployeesFilter)
        oParameters.Add("xBeginDate", xBeginPeriod.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("xEndDate", xEndPeriod.ToString("yyyy/MM/dd HH:mm"))

        oParameters.Add("CopyMainShifts", bolCopyMainShifts)
        oParameters.Add("CopyAlternativeShifts", bolCopyAlternativeShifts)
        oParameters.Add("CopyHolidays", bolCopyHolidays)

        oParameters.Add("KeepBloquedDays", bolKeepBloquedDays)
        oParameters.Add("KeepHolidaysDays", bolKeepHolidays)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.CopyPlan, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function CopyAdvShiftsEmployeesInBackground(ByVal intSourceIDEmployee As ArrayList, ByVal lstDestinationEmployees As ArrayList, ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal strShiftsSelected As String,
                                            ByVal bolCopyMainShifts As Boolean, ByVal bolCopyAlternativeShifts As Boolean, ByVal bolCopyHolidays As Boolean,
                                            ByVal bolKeepBloquedDays As Boolean, ByVal bolKeepHolidays As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        Dim sourceEmployees As String = ""
        If intSourceIDEmployee.Count > 0 Then sourceEmployees = Strings.Join(intSourceIDEmployee.ToArray(), ",")

        Dim destEmployees As String = ""
        If lstDestinationEmployees.Count > 0 Then destEmployees = Strings.Join(lstDestinationEmployees.ToArray(), ",")

        oParameters.Add("lstOriginEmployees", sourceEmployees)
        oParameters.Add("lstDestEmployees", destEmployees)
        oParameters.Add("xBeginDate", xBeginDate.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("xEndDate", xEndDate.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("strSelectedShifts", strShiftsSelected)

        oParameters.Add("CopyMainShifts", bolCopyMainShifts)
        oParameters.Add("CopyAlternativeShifts", bolCopyAlternativeShifts)
        oParameters.Add("CopyHolidays", bolCopyHolidays)

        oParameters.Add("KeepBloquedDays", bolKeepBloquedDays)
        oParameters.Add("KeepHolidaysDays", bolKeepHolidays)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.CopyAdvancedPlan, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function AssignWeekShiftsInBackground(ByVal strDestinationEmployeesFilter As String, ByVal lstWeekShifts As ArrayList, ByVal lstWeekStartShifts As Generic.List(Of DateTime),
                                     ByVal lstWeekAssignments As Generic.List(Of Integer), ByVal xBeginDate As Date, ByVal xEndDate As Date,
                                     ByVal bolKeepBloquedDays As Boolean, ByVal bolKeepHolidaysDays As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        Dim strWeekStartShifs As String = ""
        For Each oDate As DateTime In lstWeekStartShifts
            strWeekStartShifs = strWeekStartShifs & oDate.ToString("yyyy/MM/dd HH:mm") & ","
        Next

        Dim strAssignments As String = ""
        For Each iID As Integer In lstWeekAssignments
            strAssignments = strAssignments & iID & ","
        Next

        If strAssignments.EndsWith(",") Then strAssignments = strAssignments.Substring(0, strAssignments.Length - 1)
        If strWeekStartShifs.EndsWith(",") Then strWeekStartShifs = strWeekStartShifs.Substring(0, strWeekStartShifs.Length - 1)

        Dim destShifts As String = ""
        If lstWeekShifts.Count > 0 Then destShifts = Strings.Join(lstWeekShifts.ToArray(), ",")

        oParameters.Add("lstEmployees", strDestinationEmployeesFilter)
        oParameters.Add("lstWeekShifts", destShifts)
        oParameters.Add("lstWeekStartShifts", strWeekStartShifs)
        oParameters.Add("lstWeekAssignments", strAssignments)
        oParameters.Add("xBeginDate", xBeginDate.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("xEndDate", xEndDate.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("KeepBloquedDays", bolKeepBloquedDays)
        oParameters.Add("KeepHolidaysDays", bolKeepHolidaysDays)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.AssignWeekPlan, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function AssignScheduleTemplateInBackground(ByVal _IDTemplate As Integer, ByVal _IDGroup As Integer, ByVal _UserFieldConditions As Generic.List(Of roUserFieldCondition), ByVal _Year As Integer, ByVal _IDShift As Integer, ByVal _StartShift As DateTime, ByVal _LockDays As Boolean, ByVal _IncludeChildGroups As Boolean, ByVal _KeepBloquedDays As Boolean, ByVal _KeepHolidays As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        If _UserFieldConditions IsNot Nothing AndAlso _UserFieldConditions.Count > 0 Then
            Dim uCondition As roUserFieldCondition
            uCondition = _UserFieldConditions(0)
            oParameters.Add("UserFieldConditionExist", True)
            oParameters.Add("UserFieldConditionCompare", roTypes.Any2Integer(uCondition.Compare))
            oParameters.Add("UserFieldConditionType", roTypes.Any2Integer(uCondition.ValueType))
            oParameters.Add("UserFieldConditionValue", uCondition.Value)
            oParameters.Add("UserFieldConditionUFieldName", uCondition.UserField.FieldName)
            oParameters.Add("UserFieldConditionUFieldType", roTypes.Any2Integer(uCondition.UserField.FieldType))
        Else
            oParameters.Add("UserFieldConditionExist", False)
        End If

        oParameters.Add("IDTemplate", _IDTemplate)
        oParameters.Add("IDGroup", _IDGroup)
        oParameters.Add("Year", _Year)
        oParameters.Add("IDShift", _IDShift)
        oParameters.Add("StartShift", _StartShift.ToString("yyyy/MM/dd HH:mm"))

        oParameters.Add("LockDays", _LockDays)

        oParameters.Add("IncludeSubGroups", _IncludeChildGroups)

        oParameters.Add("KeepBloquedDays", _KeepBloquedDays)
        oParameters.Add("KeepHolidaysDays", _KeepHolidays)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.AssignTemplate, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function AssignScheduleTemplatev2InBackground(ByVal _IDTemplate As Integer, ByVal iIDEmployeeDestination As ArrayList, ByVal _IDShift As Integer, ByVal _StartShift As DateTime, ByVal _LockDays As Boolean, ByVal _KeepBloquedDays As Boolean, ByVal _KeepHolidays As Boolean, ByVal oState As roWsState, ByVal _FeastDays As Boolean, ByVal _EmployeeFilter As String, ByVal _IdUserField As String, ByVal _TxtUserField As String) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        Dim strDestIDEmployees As String = ""
        For Each iID As Integer In iIDEmployeeDestination
            strDestIDEmployees = strDestIDEmployees & iID & ","
        Next
        If strDestIDEmployees.EndsWith(",") Then strDestIDEmployees = strDestIDEmployees.Substring(0, strDestIDEmployees.Length - 1)
        oParameters.Add("lstDestEmployees", strDestIDEmployees)
        oParameters.Add("lstEmployees", strDestIDEmployees)

        oParameters.Add("IDTemplate", _IDTemplate)
        oParameters.Add("IDShift", _IDShift)
        oParameters.Add("StartShift", _StartShift.ToString("yyyy/MM/dd HH:mm"))

        oParameters.Add("LockDays", _LockDays)
        oParameters.Add("FeastDays", _FeastDays)

        oParameters.Add("KeepBloquedDays", _KeepBloquedDays)
        oParameters.Add("KeepHolidaysDays", _KeepHolidays)
        oParameters.Add("lstEmployeesFilter", _EmployeeFilter)
        oParameters.Add("UserFieldName", _IdUserField)
        oParameters.Add("UserFieldValue", _TxtUserField)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.AssignTemplatev2, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function CreateCalendarSpecialPaste(ByVal _copyStartDate As DateTime, ByVal _copyEndDate As DateTime, ByVal _DestStartDate As DateTime, ByVal iIDEmployeeSource As Integer, ByVal iIDEmployeeDestination As Generic.List(Of Integer),
                                               ByVal iRepeatMode As Integer, ByVal strRepeatModeValue As String, ByVal iRepeatStartMode As Integer, ByVal strRepeatStartModeValue As String,
                                               ByVal iRepeatSkipMode As Integer, ByVal iRepeatSkiptimes As Integer, ByVal strRepeatSkipModeValue As String, ByVal iBloquedMode As Integer, ByVal iHolidaysMode As Integer,
                                               ByVal iCopyMainShifts As Integer, ByVal iCopyHolidays As Integer, ByVal iLockDestDays As Integer, ByVal iCopyAssignment As Integer, ByVal iTelecommuteCopyOption As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        oParameters.Add("FromDateDestination", _DestStartDate.ToString("yyyy/MM/dd HH:mm"))

        oParameters.Add("BeginDateSource", _copyStartDate.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("EndDateSource", _copyEndDate.ToString("yyyy/MM/dd HH:mm"))

        oParameters.Add("lstOriginEmployees", iIDEmployeeSource)

        Dim strDestIDEmployees As String = ""
        For Each iID As Integer In iIDEmployeeDestination
            strDestIDEmployees = strDestIDEmployees & iID & ","
        Next
        If strDestIDEmployees.EndsWith(",") Then strDestIDEmployees = strDestIDEmployees.Substring(0, strDestIDEmployees.Length - 1)

        oParameters.Add("lstDestEmployees", strDestIDEmployees)

        oParameters.Add("RepeatMode", iRepeatMode)
        oParameters.Add("RepeatModeValue", strRepeatModeValue)
        oParameters.Add("RepeatStartMode", iRepeatStartMode)
        oParameters.Add("RepeatStartModeValue", strRepeatStartModeValue)
        oParameters.Add("RepeatSkipMode", iRepeatSkipMode)
        oParameters.Add("RepeatSkipTimes", iRepeatSkiptimes)
        oParameters.Add("RepeatSkipModeValue", strRepeatSkipModeValue)
        oParameters.Add("BlockedMode", iBloquedMode)
        oParameters.Add("HolidaysMode", iHolidaysMode)

        oParameters.Add("CopyMainShifts", iCopyMainShifts)
        oParameters.Add("CopyHolidays", iCopyHolidays)
        oParameters.Add("LockDestDays", iLockDestDays)

        oParameters.Add("CopyAssignment", iCopyAssignment)
        oParameters.Add("CopyTelecommute", iTelecommuteCopyOption)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.CopyAdvancedPlanv2, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function CreateBudgetSpecialPaste(ByVal _copyStartDate As DateTime, ByVal _copyEndDate As DateTime, ByVal _DestStartDate As DateTime, ByVal iIDNodeSource As Integer, ByVal iIDProductiveUnitSource As Integer,
                                               ByVal iRepeatMode As Integer, ByVal iHolidaysMode As Integer, ByVal strRepeatModeValue As String, ByVal bolCopyEmployees As Boolean,
                                                ByVal oState As roWsState) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        oParameters.Add("FromDateDestination", _DestStartDate.ToString("yyyy/MM/dd HH:mm"))

        oParameters.Add("BeginDateSource", _copyStartDate.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("EndDateSource", _copyEndDate.ToString("yyyy/MM/dd HH:mm"))

        oParameters.Add("lstOriginNode", iIDNodeSource)
        oParameters.Add("lstOriginProductiveUnit", iIDProductiveUnitSource)

        oParameters.Add("RepeatMode", iRepeatMode)
        oParameters.Add("RepeatModeValue", strRepeatModeValue)

        oParameters.Add("KeepHolidays", iHolidaysMode = 0)

        oParameters.Add("CopyEmployees", bolCopyEmployees)

        oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.CopyAdvancedBudgetPlan, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function CreateImportBackground(ByVal IDImport As Integer, ByVal oFileOrig() As Byte, ByVal oFileSchema() As Byte, ByVal EmployeesSelected As String, ByVal ScheduleBeginDate As Date, ByVal ScheduleEndDate As Date, ByVal oState As roWsState,
                                           ByVal ExcelIsTemplate As Integer, ByVal CopyMainShifts As Boolean, ByVal CopyHolidays As Boolean, ByVal KeepHolidays As Boolean, ByVal KeepLockedDays As Boolean, ByVal separator As String, ByVal fileType As Integer, Optional ByVal iIDImportTemplate As Integer = -1) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        ' Inicializa LastLog de la tarea

        Try
            Dim strQuery As String = "@UPDATE# ImportGuides set LastLog='' where id=" & IDImport

            Dim oSqlCommand As DbCommand = CreateCommand(strQuery)
            Dim nRet As Integer = oSqlCommand.ExecuteNonQuery()
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "LiveTasksService::CreateImportBackground:UpdateImportGuides")
        Finally

        End Try

        ' Crea la tarea de importación
        Dim ret As Integer = -1
        Try
            Dim oParameters As New roCollection

            oParameters.Add("IDImport", IDImport)

            Dim originalFileName As String = String.Empty
            Dim schemaFileName As String = String.Empty

            ' Azure. Guardo fichero de importación en un Blob container del cliente.
            ' TODO: El fichero de datos puede ser un ASCII. Se debería quitar la extensión (no es necesaria)
            originalFileName = "TMPPostProcess_#" & oState.IDPassport & "#" & Now.Ticks.ToString & ".xlsx"
            Robotics.Azure.RoAzureSupport.SaveFileOnAzure(oFileOrig, originalFileName, DTOs.roLiveQueueTypes.datalink)

            ' Guardo el fichero de schema si existe
            If oFileSchema IsNot Nothing AndAlso oFileSchema.Length > 0 Then
                schemaFileName = "TMPSchema_#" & oState.IDPassport & "#" & Now.Ticks.ToString & ".xlsx"
                Robotics.Azure.RoAzureSupport.SaveFileOnAzure(oFileSchema, schemaFileName, DTOs.roLiveQueueTypes.datalink)
            End If

            oParameters.Add("oFileOrig", originalFileName)
            oParameters.Add("oFileSchema", schemaFileName)
            oParameters.Add("IDImportTemplate", iIDImportTemplate)
            oParameters.Add("EmployeesSelected", EmployeesSelected)
            oParameters.Add("ScheduleBeginDate", ScheduleBeginDate.ToString("yyyy/MM/dd HH:mm"))
            oParameters.Add("ScheduleEndDate", ScheduleEndDate.ToString("yyyy/MM/dd HH:mm"))
            oParameters.Add("ExcelIsTemplate", ExcelIsTemplate)
            oParameters.Add("CopyMainShifts", CopyMainShifts)
            oParameters.Add("CopyHolidays", CopyHolidays)
            oParameters.Add("KeepHolidays", KeepHolidays)
            oParameters.Add("KeepLockedDays", KeepLockedDays)
            oParameters.Add("Separator", separator)
            oParameters.Add("FileType", fileType)

            ' Scripts de pre y post proceso
            Dim oDataLinkState As New roDataLinkState(bState.IDPassport)
            Dim oImportGuide As New DataLink.roImportGuide(IDImport, oDataLinkState)

            If Not oImportGuide Is Nothing Then
                Try
                    Dim oImportGuideTemplate As New DataLink.roImportGuideTemplate
                    If oImportGuideTemplate.LoadByParentId(oImportGuide.ID, iIDImportTemplate) Then
                        oParameters.Add("PreProcessScript", oImportGuideTemplate.PreProcessScript)
                        oParameters.Add("PostProcessScript", oImportGuideTemplate.PostProcessScript)
                        If oImportGuide.Version = 2 Then
                            oParameters.Add("ExcelTemplateFile", oImportGuideTemplate.Profile)
                        End If
                    End If
                Catch ex As Exception
                End Try
            End If

            oResult.Value = roLiveTask.CreateLiveTask(roLiveTaskTypes.Import, oParameters, bState)
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "LiveTasksService::CreateImportBackground")
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function CreateExportBackground(ByVal IDExport As Integer, ByVal IDTemplate As Integer, ByVal FileType As String, ByVal EmployeesSelected As String, ByVal inExcel2007Format As Boolean, ByVal ScheduleBeginDate As Date, ByVal ScheduleEndDate As Date,
                                           ByVal isExcelInstalled As Boolean, ByVal ExcelTemplateFile As String, ByVal ConceptGroup As String, ByVal Separator As String, ByVal ProfileType As Integer, ByVal bApplyLockDate As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oParameters As New roCollection

        Dim intLiveTaskType As Integer = roLiveTaskTypes.Export

        oParameters.Add("IDExport", IDExport)
        oParameters.Add("IDExportTemplate", IDTemplate)
        oParameters.Add("FileType", FileType)
        oParameters.Add("EmployeesSelected", EmployeesSelected)
        oParameters.Add("inExcel2007Format", inExcel2007Format)
        oParameters.Add("isExcelInstalled", isExcelInstalled)
        oParameters.Add("ExcelTemplateFile", ExcelTemplateFile)
        oParameters.Add("ScheduleBeginDate", ScheduleBeginDate.ToString("yyyy/MM/dd HH:mm"))
        oParameters.Add("ScheduleEndDate", ScheduleEndDate.ToString("yyyy/MM/dd HH:mm"))
        If IsNothing(ConceptGroup) Then ConceptGroup = ""
        oParameters.Add("ConceptGroup", ConceptGroup)
        If IsNothing(Separator) Then Separator = ""
        oParameters.Add("Separator", Separator)
        oParameters.Add("ProfileType", ProfileType)
        oParameters.Add("ApplyLockDate", bApplyLockDate)

        Dim oDataLinkState As New roDataLinkState(-1)
        Dim oExportGuide As New DataLink.roExportGuide(IDExport, oDataLinkState)

        If Not oExportGuide Is Nothing Then
            ' Si es una exportación de tipo 2, busco la plantilla y posibles scripts de postproceso
            ' Para cualquier tipo de plantilla, busco si hay scripts de postproceso (es una estandarización de los ya existentes, que se indicaban en la propia plantilla)
            Try
                Dim oExportGuideTemplate As New DataLink.roExportGuideTemplate
                If oExportGuideTemplate.LoadByParentId(oExportGuide.ID, IDTemplate) Then
                    oParameters.Add("PreProcessScript", oExportGuideTemplate.PreProcessScript)
                    oParameters.Add("PostProcessScript", oExportGuideTemplate.PostProcessScript)
                    If oExportGuide.Version = 2 Then
                        oParameters.Add("ExcelTemplateFile", oExportGuideTemplate.Profile)
                    End If
                End If
            Catch ex As Exception
            End Try
        End If

        If ProfileType = 999 Then
            ' Si es una exportación personalizada a un WS externo, indicamos la action CustomExport y el EndPointAdress correspondiente
            intLiveTaskType = roLiveTaskTypes.CustomExport
            Dim EndpointAddress As String = ""
            If Not oExportGuide Is Nothing Then EndpointAddress = oExportGuide.ExportFileName
            oParameters.Add("EndpointAddress", EndpointAddress)
        End If

        oResult.Value = roLiveTask.CreateLiveTask(intLiveTaskType, oParameters, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DownloadFile(ByVal fileName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Byte())

        Dim oResult As New roGenericVtResponse(Of Byte())
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim arrFile As Byte() = Nothing

        Try
            If IO.File.Exists(fileName) Then
                arrFile = IO.File.ReadAllBytes(fileName)
            Else
                Dim oSettings As New roSettings()
                Dim strExcelFileName As String = oSettings.GetVTSetting(eKeys.DataLink) & "\" & fileName
                If IO.File.Exists(strExcelFileName) Then arrFile = IO.File.ReadAllBytes(strExcelFileName)
            End If
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "roImportGuide::GetExportTemplates")
        Finally
        End Try

        oResult.Value = arrFile

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DownloadFileAzure(ByVal fileName As String, ByVal Container As roLiveQueueTypes, ByVal oState As roWsState) As roGenericVtResponse(Of Byte())

        Dim oResult As New roGenericVtResponse(Of Byte())
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim arrFile As Byte() = Nothing

        Try
            arrFile = RoAzureSupport.DownloadFile(fileName, Container)
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "LiveTasksService::DownloadFileAzure")
        Finally
        End Try

        oResult.Value = arrFile

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DownloadCommonTemplateAzure(ByVal fileName As String, ByVal Container As roLiveQueueTypes, ByVal oState As roWsState) As roGenericVtResponse(Of Byte())

        Dim oResult As New roGenericVtResponse(Of Byte())
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim arrFile As Byte() = Nothing

        Try
            arrFile = RoAzureSupport.GetCommonTemplateBytesFromAzure(fileName, Container)
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "LiveTasksService::DownloadFileAzure")
        Finally
        End Try

        oResult.Value = arrFile

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DownloadFileAzureToFile(ByVal fileName As String, ByVal destfileName As String, ByVal Container As roLiveQueueTypes, ByVal oState As roWsState) As roGenericVtResponse(Of Byte())

        Dim oResult As New roGenericVtResponse(Of Byte())
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim arrFile As Byte() = Nothing

        Try
            arrFile = RoAzureSupport.DownloadFileToFile(fileName, destfileName, Container)
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "LiveTasksService::DownloadFileAzureToFile")
        Finally
        End Try

        oResult.Value = arrFile

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DownloadFileZippedFromAzure(ByVal fileName As String, ByVal Container As roLiveQueueTypes, ByVal oState As roWsState) As roGenericVtResponse(Of Byte())
        Dim oResult As New roGenericVtResponse(Of Byte())

        'cambio mi state genérico a un estado especifico
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim arrFile As Byte() = Nothing

        Dim dmodified As DateTime = RoAzureSupport.GetLastModified(fileName, "", Container)

        If dmodified <> Date.MinValue Then
            arrFile = RoAzureSupport.DownloadFile(fileName, Container)
        Else
            arrFile = RoAzureSupport.DownloadFile(fileName.Replace(RoAzureSupport.GetCompanyName(), "export"), Container, True, RoAzureSupport.GetCompanyName())
        End If

        If arrFile IsNot Nothing Then
            Try
                Dim stream = New System.IO.MemoryStream()
                Dim zipStream = New System.IO.Compression.DeflateStream(stream, System.IO.Compression.CompressionMode.Compress, True)
                zipStream.Write(arrFile, 0, arrFile.Length)
                zipStream.Close()
                oResult.Value = stream.ToArray()

            Catch ex As Exception
                bState.UpdateStateInfo(ex, "LiveTasksService::DownloadFileZippedFromAzure")
            End Try
        Else
            bState.Result = LiveTasksResultEnum.FileNotFound
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function UploadFile(ByVal fileName As String, ByVal fileConent As Byte(), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim bolRet As Boolean = False

        Try
            System.IO.File.WriteAllBytes(fileName, fileConent)
            bolRet = True
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "roImportGuide::GetExportTemplates")
            bolRet = False
        End Try

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DuplicateFile(ByVal fileName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim bolRet As Boolean = False

        Try
            Dim originalExtension As String = System.IO.Path.GetExtension(fileName)
            Dim originalPath As String = fileName.Replace(System.IO.Path.GetFileName(fileName), "")
            Dim oldFileName As String = System.IO.Path.GetFileNameWithoutExtension(fileName)
            Dim index As Integer = 0

            Dim newFileName As String = ""

            Dim fileNameParts As String() = fileName.Split("_")
            If fileNameParts.Length = 3 Then
                newFileName = fileNameParts(0) & "_" & fileNameParts(1) & "_" & index & originalExtension
            Else
                newFileName = originalPath & oldFileName & "_" & index & originalExtension
            End If

            While System.IO.File.Exists(newFileName)
                index = index + 1
                newFileName = fileNameParts(0) & "_" & fileNameParts(1) & "_" & index & originalExtension
            End While

            System.IO.File.WriteAllBytes(newFileName, System.IO.File.ReadAllBytes(fileName))
            bolRet = True
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "roImportGuide::GetExportTemplates")
            bolRet = False
        Finally
        End Try

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function RemoveFile(ByVal fileName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim bolRet As Boolean = False

        Try
            System.IO.File.Delete(fileName)
            bolRet = True
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "roImportGuide::GetExportTemplates")
            bolRet = False
        Finally
        End Try

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function FileExists(ByVal fileName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roLiveTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim bolRet As Boolean = False

        Try
            bolRet = System.IO.File.Exists(fileName)
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "roImportGuide::GetExportTemplates")
            bolRet = False
        Finally
        End Try

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class