Imports System.Data.SqlClient
Imports System.Web.Mvc
Imports DevExpress.DataAccess.ConnectionParameters
Imports DevExpress.DataProcessing
Imports DevExpress.Web.Mvc
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports Robotics.Web.VTLiveMvC.Controllers

<PermissionsAtrribute(FeatureAlias:="Reports", Permission:=Robotics.Base.DTOs.Permission.Write)>
<LoggedInAtrribute(Requiered:=True)>
Public Class ReportController
    Inherits System.Web.Mvc.Controller

    Private oLanguage As roLanguageWeb

    Public Function ScriptsVersion() As String
        Return "?v=" & Robotics.VTBase.roTypes.Any2String(Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(Reflection.AssemblyDescriptionAttribute), False).FirstOrDefault().Description)
    End Function

    Function Index() As ActionResult
        Return View(FilterReports(New ReportsController().GetReportsDataView()))
    End Function

    Function CardView() As ActionResult
        Return PartialView("cardView", FilterReports(New ReportsController().GetReportsDataView()))
    End Function

    Function ReportDesigner() As ActionResult
        Return View("ReportDesigner", New ReportsController().OpenReportDesigner())
    End Function

    Public Function OpenReportDesigner(Id As Integer) As ActionResult
        ViewBag.SyncOrAsync = "Synchronous"
        Dim remoteController As ReportsController = New ReportsController()

        If remoteController.GetReportById(Id, ReportPermissionTypes.Update) IsNot Nothing Then
            Return View("ReportDesigner", remoteController.OpenReportDesigner(Id))
        Else
            Return Nothing
        End If
    End Function

    Function FilterReports(reportsList As List(Of Report)) As List(Of Report)
        Dim filteredReports = reportsList.Where(Function(oReport) Not oReport.Name.StartsWith("Subreport")).OrderBy(Function(f) f.Name)

        For Each oReport In filteredReports
            If (IsNothing(oReport.CreatorPassportId)) Then
                oReport.DisplayName = GetServerLanguage().TranslateWithDefault("reportname", oReport.Name.Replace(" ", ""), oReport.Name)
                oReport.Description = GetServerLanguage().TranslateWithDefault("reportdescription", oReport.Name.Replace(" ", ""), "")
            Else
                oReport.DisplayName = oReport.Name
            End If
            If oReport.DisplayName.Contains(":") Then
                oReport.DisplayName = oReport.DisplayName.Substring(0, oReport.DisplayName.IndexOf(":"))
            End If
        Next

        Return filteredReports.ToList
    End Function

    <HttpPost>
    Function GetViewTemplate(templateName As String, idReport As Integer) As ActionResult
        If templateName = "selectorEmployees" Then

            Dim oReport = New ReportsController().GetReportById(idReport, ReportPermissionTypes.Read)
            If oReport IsNot Nothing AndAlso (IsNothing(oReport.RequieredFeature)) Then
                oReport.RequieredFeature = "U:Employees=Read"
            End If

            Return PartialView(templateName, oReport)
        Else
            Return PartialView(templateName, {}) ' New ReportsController().GetReportsDataView())
        End If

    End Function

    <HttpPost>
    Public Function GetReportByIdAsJson(reportId As Integer) As String
        Dim oReport = New ReportsController().GetReportById(reportId, ReportPermissionTypes.Read)

        If oReport.CreatorPassportId.HasValue = False Then
            oReport.DisplayName = GetServerLanguage().Translate("reportname", oReport.Name.Replace(" ", ""))
            oReport.Description = GetServerLanguage().Translate("reportdescription", oReport.Name.Replace(" ", ""))
        Else
            oReport.DisplayName = oReport.Name
            oReport.Description = oReport.Description
        End If

        Return roJSONHelper.SerializeNewtonSoft(oReport)
    End Function

    <HttpPost>
    Public Function GetExportDataAndExtension(executionId As Guid) As FileContentResult
        Dim fileContents = New ReportsController().GetExportDataAndExtension(executionId)
        Return File(fileContents, "application/octet-stream")
    End Function

    <HttpPost>
    Public Function GetReportCategoriesAsJson() As String
        Return roJSONHelper.SerializeNewtonSoft(GetAvailableCategories())
    End Function

    Public Function GetAvailableCategories() As List(Of roSecurityCategory)
        Return SecurityV3ServiceMethods.GetRequestCategories(Nothing)
    End Function

    <HttpPost>
    Public Function GetUserPassportID() As Integer
        Return WLHelperWeb.CurrentPassportID
    End Function

    <HttpPost>
    Public Function GetReportsPageConfiguration() As String
        Return New ReportsController().GetReportsPageConfiguration()
    End Function

    'Public Function GetReportCategories() As List(Of ReportCategory)
    '    Return New ReportsController().GetReportCategories()
    'End Function

    Public Function GetServerLanguage() As roLanguageWeb
        If Me.oLanguage Is Nothing Then
            Me.oLanguage = New roLanguageWeb
            WLHelperWeb.SetLanguage(Me.oLanguage, "LiveDXReport")
        End If
        Return Me.oLanguage
    End Function

    Public Function GetReportDesignerConnectionParameters() As MsSqlConnectionParameters
        Dim strConn As String = API.UserAdminServiceMethods.GetConnectionString(Nothing, WLHelperWeb.CurrentPassportID)

        Dim builder As New SqlConnectionStringBuilder(strConn)
        Return New MsSqlConnectionParameters(builder.DataSource, builder.InitialCatalog, builder.UserID, builder.Password, MsSqlAuthorizationType.SqlServer)
    End Function

    <HttpPost>
    Public Function GetExecutionStatus(executionId As Guid) As Integer
        Return New ReportsController().GetExecutionStatus(executionId)
    End Function

    <HttpPost>
    Public Function TriggerExecution(reportId As Integer, Optional reportParameters As String = "", Optional viewFields As String = "") As Integer
        Return New ReportsController().TriggerExecution(reportId, reportParameters, viewFields)
    End Function

    <HttpPost>
    Public Function GetUsers() As String
        Return roJSONHelper.SerializeNewtonSoft(New ReportsController().GetUsers())
    End Function

    <HttpPost>
    Public Function SaveReportInfo(reportData As String, flag As String) As Boolean
        Return New ReportsController().SaveReportInfo(reportData, flag).ToString()
    End Function

    <HttpPost>
    Public Function DeleteReportLayout(reportId As Integer) As Boolean
        Dim remoteController = New ReportsController()
        Dim reportObject As Report = remoteController.GetReportById(reportId, ReportPermissionTypes.Delete)
        If reportObject IsNot Nothing Then Return remoteController.DeleteReportLayout(reportId).ToString()

        Return False
    End Function

    <HttpPost>
    Public Function GetSelectorUniversalParamOptions(paramType As String, isEmergencyReport As Boolean) As String
        Return New ReportsController().GetGenericParameterSelectorOptions(paramType, isEmergencyReport)
    End Function

    <HttpPost>
    Public Function GetAllLanguages(paramType As String) As String
        Dim oLanguages = API.SecurityServiceMethods.GetLanguages(Nothing).Where(Function(lang) lang.Installed)
        Return roJSONHelper.SerializeNewtonSoft(oLanguages)
    End Function

    <HttpPost>
    Public Function CopyReport(reportId As Integer, newReportName As String) As Boolean
        Dim remoteController = New ReportsController()
        Return remoteController.CopyReport(reportId, newReportName)
    End Function

    <HttpPost>
    Public Function UpdateReportCategories(reportId As Integer, categoryId As Integer, categoryLevel As Integer) As Boolean
        Dim remoteController = New ReportsController()
        Dim oLst As New List(Of (Integer, Integer))
        oLst.Add((categoryId, categoryLevel))
        Return remoteController.UpdateReportCategories(reportId, oLst)
    End Function

    Public Function GetRedirectPath() As String
        Return "window.parent.reenviaFrame('" & String.Format("/{0}//Report", Configuration.RootUrl) & "', '','Informes','Portal\Reports',@@mvcPath@@)"
    End Function

End Class