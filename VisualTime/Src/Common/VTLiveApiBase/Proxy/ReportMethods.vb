Imports System.IO
Imports ReportGenerator.Services
Imports Robotics.Azure
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTReports.Services
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports VTReports.Domain

Public Class ReportMethods

#Region "Shared Methods"

    Private Shared Function UpdateInitialStateInfo(ByRef oState As roWsState) As roReportsState
        Dim bState As New roReportsState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()
        Return bState
    End Function

    Private Shared Sub UpdateFinalStateInfo(ByVal oState As roReportsState, ByRef resultStatus As roWsState)
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oState, newGState)
        resultStatus = newGState
    End Sub

    Private Shared Sub SetUserNotLoggedState(ByRef resultStatus As roWsState, ByRef resultValue As Object)
        resultStatus = New roWsState With {
                .ReturnCode = ReportResultEnum.Exception
            }
        resultValue = Nothing
    End Sub

#End Region

#Region "DevExpress Reports"

    Public Shared Function GetReportByName(reportName As String, ByVal action As ReportPermissionTypes, ByVal passportId As Integer, oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Report)
        Dim oResult As New roGenericVtResponse(Of Report)

        Dim bState As roReportsState = UpdateInitialStateInfo(oState)

        Dim reportService = New ReportService()
        oResult.Value = reportService.GetReportByName(reportName, passportId, oState:=bState, bAudit:=bolAudit)

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

    Public Shared Function GetReportsSimplified(passportId As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Report))
        Dim oResult = New roGenericVtResponse(Of List(Of Report))

        Dim bState As roReportsState = UpdateInitialStateInfo(oState)

        Dim reportService = New ReportService()
        oResult.Value = reportService.GetReportsByPassportIdSimplified(passportId, bState, False).ToList()

        UpdateFinalStateInfo(bState, oResult.Status)
        Return oResult
    End Function

    Public Shared Function GetReportById(ByVal reportId As Integer, ByVal action As ReportPermissionTypes, ByVal passportId As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Report)
        Dim oResult = New roGenericVtResponse(Of Report)

        Dim bState As roReportsState = UpdateInitialStateInfo(oState)

        Dim reportService = New ReportService()
        oResult.Value = reportService.GetReportById(reportId, passportId, bState, False)

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

    Public Shared Function GetExecutionStatus(ByVal executionId As Guid, ByVal oState As roWsState) As roGenericVtResponse(Of ReportExecution)
        Dim oResult = New roGenericVtResponse(Of ReportExecution)

        Dim bState As roReportsState = UpdateInitialStateInfo(oState)

        Dim reportExecutionService = New ReportExecutionService()
        oResult.Value = reportExecutionService.GetExecution(executionId)

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

    Public Shared Function GetReportsPageConfiguration(ByVal passportId As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String)
        Dim oResult = New roGenericVtResponse(Of String)

        Dim bState As roReportsState = UpdateInitialStateInfo(oState)

        Dim reportExecutionService = New ReportExecutionService()

        Dim configurationDictionary As IDictionary(Of String, String) = New Dictionary(Of String, String) From {
                {"numberOfMaximumExecutions", reportExecutionService.GetNumberOfMaximumExecutions().ToString},
                {"reportManagerPermissionByUser", WLHelper.GetPermissionOverFeature(passportId, "Reports", "U")}
            }

        oResult.Value = roJSONHelper.SerializeNewtonSoft(configurationDictionary)

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

    Public Shared Function GetExecutionAssociatedExportDataAndExtension(ByVal executionId As Guid, ByVal passportId As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of (Byte(), String))
        Dim oResult = New roGenericVtResponse(Of (Byte(), String))

        Dim bState As roReportsState = UpdateInitialStateInfo(oState)

        Dim permissionService As IReportPermissionService = New ReportPermissionServiceV3()

        If permissionService.GetGeneralReportPermission(passportId, ReportPermissionTypes.Read) Then
            Dim reportExecutionService = New ReportExecutionService()
            Dim reportExecution As ReportExecution = reportExecutionService.GetExecution(executionId)

            Try
                oResult.Value = (RoAzureSupport.DownloadFile(RoAzureSupport.GetCompanyName() & "/" & reportExecution.FileLink, roLiveQueueTypes.reports), reportExecution.Extension)
            Catch ex As Exception
                oResult.Value = ({}, String.Empty)
            End Try
        End If

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

    Public Shared Function GetEmergencyReport(passportId As Integer, oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Report)
        Dim oResult = New roGenericVtResponse(Of Report)

        Dim bState As roReportsState = UpdateInitialStateInfo(oState)

        Dim reportService = New ReportService()
        oResult.Value = reportService.GetEmergencyReport(passportId, bState, False)

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

    Public Shared Function ExecuteEmergencyReport(passportId As Integer, oState As roWsState, bolAudit As Boolean, Optional strReports As String = Nothing) As roGenericVtResponse(Of Boolean)
        Dim oResult = New roGenericVtResponse(Of Boolean)

        Dim bState As roReportsState = UpdateInitialStateInfo(oState)

        Dim reportService = New ReportService()
        oResult.Value = reportService.ExecuteEmergencyReport(passportId, bState, bolAudit, strReports)

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

    Public Shared Function SaveReportLastParameters(lastParameters As String, reportId As Integer, passportId As String, oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim bState As roReportsState = UpdateInitialStateInfo(oState)

        Dim parametersService = New ReportParameterService()
        oResult.Value = parametersService.SaveLastParameters(reportId, passportId, lastParameters)

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

    Public Shared Function InsertReport(report As Report, ByVal action As ReportPermissionTypes, oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim bState As roReportsState = UpdateInitialStateInfo(oState)

        Dim reportService = New ReportService()
        oResult.Value = reportService.InsertReport(report, bState, bolAudit)

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

    Public Shared Function UpdateReport(report As Report, oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim bState As roReportsState = UpdateInitialStateInfo(oState)

        Dim reportService = New ReportService()
        oResult.Value = reportService.UpdateReport(report, bState, bolAudit)

        UpdateFinalStateInfo(bState, oResult.Status)
        Return oResult
    End Function

    Public Shared Function UpdateReportExecutions(reportExecutionsList As List(Of ReportExecution), reportId As Integer, passportId As Integer, oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim bState As roReportsState = UpdateInitialStateInfo(oState)

        Dim reportExecutionService = New ReportExecutionService()
        oResult.Value = reportExecutionService.UpdateExecutionsFromReport(reportExecutionsList, reportId, passportId, bState, bolAudit)

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

    Public Shared Function UpdateReportPlannedExecutions(reportPlanificationsList As List(Of ReportPlannedExecution), parametersJson As String, reportId As Integer, passportId As Integer, oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim bState As roReportsState = UpdateInitialStateInfo(oState)

        Dim reportPlanificationService = New ReportPlannedExecutionService()
        oResult.Value = reportPlanificationService.UpdatePlannedExecutionsFromReport(reportPlanificationsList, parametersJson, reportId, passportId, bState, bolAudit)

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

    Public Shared Function DeleteReport(ByVal reportId As Integer, ByVal passportId As Integer, ByVal ostate As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim oResult = New roGenericVtResponse(Of Boolean)

        Dim bState As roReportsState = UpdateInitialStateInfo(ostate)

        Dim reportLayoutservice = New ReportService()
        oResult.Value = reportLayoutservice.DeleteReport(reportId, passportId, bState, False)

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

    Public Shared Function CopyReport(ByVal reportId As Integer, ByVal newReportName As String, ByVal passportId As Integer, ByVal ostate As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim oResult = New roGenericVtResponse(Of Boolean)

        Dim bState As roReportsState = UpdateInitialStateInfo(ostate)

        Dim reportLayoutservice = New ReportService()
        oResult.Value = reportLayoutservice.CopyReport(reportId, newReportName, passportId, bState, False)

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

    Public Shared Function UpdateReportCategories(ByVal reportId As Integer, ByVal reportCategories As List(Of (Integer, Integer)), ByVal passportId As Integer, ByVal ostate As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim oResult = New roGenericVtResponse(Of Boolean)

        Dim bState As roReportsState = UpdateInitialStateInfo(ostate)

        Dim reportLayoutservice = New ReportService()
        oResult.Value = reportLayoutservice.UpdateReportCategories(reportId, reportCategories, passportId, bState, False)

        UpdateFinalStateInfo(bState, oResult.Status)

        Return oResult
    End Function

#End Region

End Class