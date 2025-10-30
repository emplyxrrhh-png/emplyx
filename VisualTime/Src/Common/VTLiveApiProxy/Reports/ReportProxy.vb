Imports System.IO
Imports System.ServiceModel.Activation
Imports ReportGenerator.Domain
Imports ReportGenerator.Services
Imports Robotics.Azure
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Report
Imports Robotics.Base.VTReports.Services
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.VTBase
Imports VTReports.Domain

Public Class ReportProxy
    Implements IReportsSvc
#Region "Shared Methods"
    Public Function KeepAlive() As Boolean Implements IReportsSvc.KeepAlive
        Return True
    End Function

#End Region

#Region "Crystal Reports"

#Region "Reports"


    Public Function GetReports(ByVal strReportType As String, ByVal oState As roWsState) As roGenericVtResponse(Of roReportList) Implements IReportsSvc.GetReports
        Return ReportMethods.GetReports(strReportType, oState)
    End Function

    Public Function GetReportsLite(ByVal strReportType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roReport)) Implements IReportsSvc.GetReportsLite
        Return ReportMethods.GetReportsLite(strReportType, oState)
    End Function

    Public Function GetReportsNames(ByVal strReportType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of String)) Implements IReportsSvc.GetReportsNames
        Return ReportMethods.GetReportsNames(strReportType, oState)
    End Function


    Public Function GetReport(ByVal strFileName As String, ByVal oState As roWsState) As roGenericVtResponse(Of roReport) Implements IReportsSvc.GetReport
        Return ReportMethods.GetReport(strFileName, oState)
    End Function


    Public Function GetLastEmergencyReportExecuted(ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IReportsSvc.GetLastEmergencyReportExecuted
        Return ReportMethods.GetLastEmergencyReportExecuted(oState)
    End Function


    Public Function GetReportProfiles(ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roReportProfileList) Implements IReportsSvc.GetReportProfiles
        Return ReportMethods.GetReportProfiles(intIDPassport, oState)
    End Function


    Public Function GetReportProfile(ByVal intIDProfile As Integer, ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roReportProfile) Implements IReportsSvc.GetReportProfile
        Return ReportMethods.GetReportProfile(intIDProfile, intIDPassport, oState)
    End Function


    Public Function SaveReportProfile(ByVal oProfile As roReportProfile, ByVal oState As roWsState) As roGenericVtResponse(Of (roReportProfile, Boolean)) Implements IReportsSvc.SaveReportProfile
        Return ReportMethods.SaveReportProfile(oProfile, oState)
    End Function


    Public Function DeleteReportProfileID(ByVal intIDProfile As Integer, ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.DeleteReportProfileID
        Return ReportMethods.DeleteReportProfileID(intIDProfile, intIDPassport, oState)
    End Function

    Public Function DeleteReportProfile(ByVal oProfile As roReportProfile, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.DeleteReportProfile
        Return ReportMethods.DeleteReportProfile(oProfile, oState)
    End Function


    Public Function GetConstants(ByVal oState As roWsState) As roGenericVtResponse(Of roReportConstants) Implements IReportsSvc.GetConstants
        Return ReportMethods.GetConstants(oState)
    End Function


    Public Function ExecuteReport(ByVal oReport As roReport, ByVal oExportFormatType As CrystalDecisions.Shared.ExportFormatType, ByVal oState As roWsState) As roGenericVtResponse(Of (roReport, Boolean)) Implements IReportsSvc.ExecuteReport
        Return ReportMethods.ExecuteReport(oReport, oExportFormatType, oState)
    End Function

    Public Function CreateTaskReport(ByVal oReport As roReport, ByVal oExportFormatType As CrystalDecisions.Shared.ExportFormatType, ByVal oState As roWsState) As roGenericVtResponse(Of Long) Implements IReportsSvc.CreateTaskReport
        Return ReportMethods.CreateTaskReport(oReport, oExportFormatType, oState)
    End Function


    Public Function GetStatusTaskReport(ByVal ID As Long, ByVal Status As Integer, ByVal UploadFile As String, ByVal oState As roWsState) As roGenericVtResponse(Of (String, Long)) Implements IReportsSvc.GetStatusTaskReport
        Return ReportMethods.GetStatusTaskReport(ID, Status, UploadFile, oState)
    End Function


    Public Function DeleteTaskReport(ByVal ID As Long, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.DeleteTaskReport
        Return ReportMethods.DeleteTaskReport(ID, oState)
    End Function


    Public Function GetEmployeeListFromFilter(ByVal sWhere As String, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IReportsSvc.GetEmployeeListFromFilter
        Return ReportMethods.GetEmployeeListFromFilter(sWhere, oState)
    End Function

#End Region

#Region "ReportsScheduler"
    ''' <summary>
    ''' Obtiene la programación con el ID indicado
    ''' </summary>
    ''' <param name="ID">ID</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve la solicitud (roReportScheduler)</returns>
    ''' <remarks></remarks>

    Public Function GetReportSchedulerByID(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roReportScheduler) Implements IReportsSvc.GetReportSchedulerByID
        Return ReportMethods.GetReportSchedulerByID(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Devuelve todas las programaciones
    ''' </summary>
    ''' <param name="SQLFilter">Filtro SQL para el Where (ejemplo: 'NType = 1 And Reque...')</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetReportSchedulers(ByVal SQLFilter As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roReportScheduler)) Implements IReportsSvc.GetReportSchedulers
        Return ReportMethods.GetReportSchedulers(SQLFilter, oState, bAudit)
    End Function

    ''' <summary>
    ''' Valida los datos de la programación<br/>
    ''' Comprueba que:<br/>
    ''' </summary>
    ''' <param name="oReportScheduler">La programación a validar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha validado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Function ValidateReportScheduler(ByVal oReportScheduler As roReportScheduler, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.ValidateReportScheduler
        Return ReportMethods.ValidateReportScheduler(oReportScheduler, oState)
    End Function

    ''' <summary>
    ''' Guarda los datos de la programación. Si és nuevo, se actualiza el ID de la solicitud pasado.<br/>
    ''' Se comprueba ValidateNotification()<br/>
    ''' </summary>
    ''' <param name="oReportScheduler">Programación a guardar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha guardado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Function SaveReportScheduler(ByVal oReportScheduler As roReportScheduler, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of (roReportScheduler, Boolean)) Implements IReportsSvc.SaveReportScheduler
        Return ReportMethods.SaveReportScheduler(oReportScheduler, oState, bAudit)
    End Function

    ''' <summary>
    ''' Elimina la programación con el ID indicado<br/>
    ''' Realiza lo siguiente:<br/>
    ''' </summary>
    ''' <param name="ID">ID a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Function DeleteReportScheduler(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.DeleteReportScheduler
        Return ReportMethods.DeleteReportScheduler(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Ejecuta un informe
    ''' </summary>
    ''' <param name="ID">ID a ejecutar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Function ExecuteReportScheduler(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.ExecuteReportScheduler
        Return ReportMethods.ExecuteReportScheduler(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Ejecuta los informes de emergencia
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Function ExecuteEmergencyReportScheduler(ByVal strReports As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.ExecuteEmergencyReportScheduler
        Return ReportMethods.ExecuteEmergencyReportScheduler(strReports, oState)
    End Function


#End Region

#Region "Printers"
    ''' <summary>
    ''' Devuelve las impresoras instaladas
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetPrinters(ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roPrinter)) Implements IReportsSvc.GetPrinters
        Return ReportMethods.GetPrinters(oState)
    End Function
#End Region


#End Region

#Region "DevExtreme Reports"
    Public Function GetReportByName(reportName As String, ByVal action As ReportPermissionTypes, ByVal passportId As Integer, oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Report) Implements IReportsSvc.GetReportByName
        Return ReportMethods.GetReportByName(reportName, action, passportId, oState, bolAudit)
    End Function

    Function GetReportsSimplified(passportId As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Report)) Implements IReportsSvc.GetReportsSimplified
        Return ReportMethods.GetReportsSimplified(passportId, oState)
    End Function

    Function GetReportById(ByVal reportId As Integer, ByVal action As ReportPermissionTypes, ByVal passportId As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Report) Implements IReportsSvc.GetReportById
        Return ReportMethods.GetReportById(reportId, action, passportId, oState)
    End Function

    Function GetExecutionStatus(ByVal executionId As Guid, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements IReportsSvc.GetExecutionStatus
        Return ReportMethods.GetExecutionStatus(executionId, oState)
    End Function

    Function GetReportsPageConfiguration(ByVal passportId As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IReportsSvc.GetReportsPageConfiguration
        Return ReportMethods.GetReportsPageConfiguration(passportId, oState)
    End Function

    Function GetExecutionAssociatedExportDataAndExtension(ByVal executionId As Guid, ByVal passportId As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of (Byte(), String)) Implements IReportsSvc.GetExecutionAssociatedExportDataAndExtension
        Return ReportMethods.GetExecutionAssociatedExportDataAndExtension(executionId, passportId, oState)
    End Function

    Public Function SaveReportLastParameters(lastParameters As String, reportId As Integer, passportId As String, oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.SaveReportLastParameters
        Return ReportMethods.SaveReportLastParameters(lastParameters, reportId, passportId, oState, bolAudit)
    End Function

    Public Function InsertReport(report As Report, ByVal action As ReportPermissionTypes, oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.InsertReport
        Return ReportMethods.InsertReport(report, action, oState, bolAudit)
    End Function

    Public Function UpdateReport(report As Report, oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.UpdateReport
        Return ReportMethods.UpdateReport(report, oState, bolAudit)
    End Function

    Public Function UpdateReportExecutions(reportExecutionsList As List(Of ReportExecution), reportId As Integer, passportId As Integer, oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.UpdateReportExecutions
        Return ReportMethods.UpdateReportExecutions(reportExecutionsList, reportId, passportId, oState, bolAudit)
    End Function

    Public Function UpdateReportPlannedExecutions(reportPlanificationsList As List(Of ReportPlannedExecution), parametersJson As String, reportId As Integer, passportId As Integer, oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.UpdateReportPlannedExecutions
        Return ReportMethods.UpdateReportPlannedExecutions(reportPlanificationsList, parametersJson, reportId, passportId, oState, bolAudit)
    End Function

    'Function UpdateEntireReport(ByVal report As Report, passportId As Integer, ByVal ostate As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.UpdateEntireReport
    '    Dim oResult = New roGenericVtResponse(Of Boolean)

    '    If Global_asax.LoggedIn Then
    '        UpdateInitialStateInfo(ostate)

    '        Using databaseConnection As DatabaseConnection = New DatabaseConnection(AccessHelper.GetConectionString())
    '            Dim reportLayoutService = New ReportService(databaseConnection)
    '            oResult.Value = reportLayoutService.UpdateEntireReport(report, passportId, bState, False)
    '        End Using

    '        UpdateFinalStateInfo(oResult.Status)
    '    Else
    '        SetUserNotLoggedState(oResult.Status, oResult.Value)
    '    End If

    '    Return oResult
    'End Function

    Function DeleteReport(ByVal reportId As Integer, ByVal passportId As Integer, ByVal ostate As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.DeleteReport
        Return ReportMethods.DeleteReport(reportId, passportId, ostate, bolAudit)
    End Function

    Function CopyReport(ByVal reportId As Integer, ByVal newReportName As String, ByVal passportId As Integer, ByVal ostate As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.CopyReport
        Return ReportMethods.CopyReport(reportId, newReportName, passportId, ostate, bolAudit)
    End Function

    Public Function GetEmergencyReport(oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Report) Implements IReportsSvc.GetEmergencyReport
        Return ReportMethods.GetEmergencyReport(Robotics.VTBase.roConstants.GetGlobalEnvironmentParameter(Robotics.VTBase.roConstants.GlobalAsaxParameter.SystemPassportID), oState, bolAudit)
    End Function

    Public Function ExecuteEmergencyReport(oState As roWsState, bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IReportsSvc.ExecuteEmergencyReport
        Return ReportMethods.ExecuteEmergencyReport(Robotics.VTBase.roConstants.GetGlobalEnvironmentParameter(Robotics.VTBase.roConstants.GlobalAsaxParameter.SystemPassportID), oState, bolAudit)
    End Function
#End Region

End Class
