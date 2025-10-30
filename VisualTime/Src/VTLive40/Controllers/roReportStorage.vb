Imports System.Data.SqlClient
Imports System.IO
Imports DevExpress.DataAccess.ConnectionParameters
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraReports.Web.Extensions
Imports Newtonsoft.Json
Imports Robotics.Base
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class roReportStorage
    Inherits ReportStorageWebExtension

    Public Overrides Function CanSetData(ByVal url As String) As Boolean
        ' Determines whether or not it is possible to store a report by a given URL.
        ' For instance, make the CanSetData method return false for reports that should be read-only in your storage.
        ' This method is called only for valid URLs (i.e., if the IsValidUrl method returned true) before the SetData method is called.
        Dim response As Boolean = False

        If WLHelperWeb.CurrentUserIsConsultantOrCegid Then
            response = True
        Else
            Dim report = ReportServiceMethods.GetReportLayoutByName(Nothing, ReportPermissionTypes.Update, url, False)

            If IsNothing(report) Then
                response = True
            Else
                If Not IsNothing(report.CreatorPassportId) Then
                    response = True
                End If
            End If
        End If

        Return response
    End Function

    Public Overrides Function IsValidUrl(ByVal url As String) As Boolean
        ' Determines whether or not the URL passed to the current Report Storage is valid.
        ' For instance, implement your own logic to prohibit URLs that contain white spaces or some other special characters.
        ' This method is called before the CanSetData and GetData methods.

        Return url.Count < 200
    End Function

    Public Overrides Function GetData(ByVal url As String) As Byte()
        ' Returns report layout data stored in a Report Storage using the specified URL.
        ' This method is called only for valid URLs after the IsValidUrl method is called.

        Dim reportLayout = ReportServiceMethods.GetReportLayoutByName(Nothing, ReportPermissionTypes.Execute, url, False)

        Dim report = XtraReport.FromStream(New MemoryStream(reportLayout.LayoutXMLBinary))

        If (report.DataSource IsNot Nothing) Then
            Dim connectionInfo = New SqlConnectionStringBuilder(API.UserAdminServiceMethods.GetConnectionString(Nothing, WLHelperWeb.CurrentPassportID))
            Dim connectionParameters = New MsSqlConnectionParameters(connectionInfo.DataSource, connectionInfo.InitialCatalog, connectionInfo.UserID, connectionInfo.Password, MsSqlAuthorizationType.SqlServer)
            report.DataSource.ConnectionParameters = connectionParameters
            report.DataSource.ConnectionOptions.CommandTimeout = 3600
        End If

        reportLayout.LayoutXMLBinary = GetCustomData(report)

        Return reportLayout.LayoutXMLBinary
    End Function

    Public Overrides Function GetUrls() As Dictionary(Of String, String)
        ' Returns a dictionary of the existing report URLs and display names.
        ' This method is called when running the Report Designer,
        ' before the Open Report and Save Report dialogs are shown and after a new report is saved to a storage.
        Dim oAvailableReports As Generic.List(Of Report) = ReportServiceMethods.GetReportsSimplified(Nothing)
        Dim oRetDictionary As New Dictionary(Of String, String)

        For Each oLayout As Report In oAvailableReports
            oRetDictionary.Add(oLayout.Name, oLayout.Name)
        Next

        Return oRetDictionary
    End Function

    Public Overrides Sub SetData(ByVal xtraReport As XtraReport, ByVal url As String)
        ' Stores the specified report to a Report Storage using the specified URL.
        ' This method is called only after the IsValidUrl and CanSetData methods are called.
        Dim report = ReportServiceMethods.GetReportLayoutByName(Nothing, ReportPermissionTypes.Update, url, False)

        report.ParametersJson = JsonConvert.SerializeObject(ConvertDevexpressParametersToReportParameters(xtraReport.Parameters))

        report.LayoutPreviewXMLBinary = GetExportImageBytes(xtraReport, report)
        report.LayoutXMLBinary = GetCustomData(xtraReport)

        If Not ReportServiceMethods.UpdateReport(Nothing, report, True) Then
            Throw New Exception("No se ha podido guardar el informe")
        End If
    End Sub

    Public Overrides Function SetNewData(ByVal xtraReport As XtraReport, ByVal defaultUrl As String) As String
        ' Stores the specified report using a new URL.
        ' The IsValidUrl and CanSetData methods are never called before this method.
        ' You can validate and correct the specified URL directly in the SetNewData method implementation
        ' and return the resulting URL used to save a report in your storage.
        If Me.CanSetData(defaultUrl) Then
            Dim report = ReportServiceMethods.GetReportLayoutByName(Nothing, ReportPermissionTypes.Update, defaultUrl, False)

            If report Is Nothing Then
                report = New Report With {
                    .Id = 0,
                    .Name = defaultUrl,
                    .Description = String.Empty,
                    .CreationDate = Date.Now,
                    .LayoutXMLBinary = {},
                    .LayoutPreviewXMLBinary = {},
                    .CreatorPassportId = IIf(WLHelperWeb.CurrentUserIsConsultantOrCegid, Nothing, WLHelperWeb.CurrentPassportID)
                    }
            End If

            report.ParametersJson = JsonConvert.SerializeObject(ConvertDevexpressParametersToReportParameters(xtraReport.Parameters))
            report.LayoutXMLBinary = GetCustomData(xtraReport)
            report.LayoutPreviewXMLBinary = GetExportImageBytes(xtraReport, report)

            If (report.Id = 0) Then
                If Not ReportServiceMethods.InsertReport(Nothing, report, ReportPermissionTypes.Update, True) Then
                    Throw New Exception("No se ha podido guardar el informe")
                End If
            Else
                If Not ReportServiceMethods.UpdateReport(Nothing, report, True) Then
                    Throw New Exception("No se ha podido guardar el informe")
                End If
            End If
        Else
            Throw New Exception("No se puede guardar este informe con este nombre")
        End If

        Return defaultUrl
    End Function

    Private Function ConvertDevexpressParametersToReportParameters(devexpressParameters As DevExpress.XtraReports.Parameters.ParameterCollection) As List(Of ReportParameter)
        Dim result = New List(Of ReportParameter)

        For Each auxiliarDXParameter As DevExpress.XtraReports.Parameters.Parameter In devexpressParameters
            result.Add(New ReportParameter With {
                                                .Name = auxiliarDXParameter.Name,
                                                .Description = auxiliarDXParameter.Description,
                                                .IsHidden = Not auxiliarDXParameter.Visible,
                                                .IsMultiValue = auxiliarDXParameter.MultiValue,
                                                .IsRangeValue = If(IsNothing(auxiliarDXParameter.Value), False, auxiliarDXParameter.Value.GetType().Name.Equals("Range`1")),
                                                .Type = auxiliarDXParameter.Type.FullName,
                                                .Value = auxiliarDXParameter.Value
                                                }
                      )
        Next

        Return result
    End Function

    Private Function GetExportImageBytes(xtraReport As XtraReport, report As Report) As Byte()
        Dim byteStream As MemoryStream = New MemoryStream()

        SetDatasourceToDevexpressReport(xtraReport)

        xtraReport.ExportOptions.Image.PageRange = "1"
        xtraReport.ExportOptions.Image.ExportMode = DevExpress.XtraPrinting.ImageExportMode.SingleFilePageByPage
        xtraReport.ExportToImage(byteStream, Drawing.Imaging.ImageFormat.Jpeg)

        UnsetDatasourceToDevexpressReport(xtraReport)

        Return byteStream.GetBuffer()
    End Function

    Private Sub SetDatasourceToDevexpressReport(xtraReport As XtraReport)
        If (xtraReport.DataSource IsNot Nothing) Then
            Dim connectionInfo = New SqlConnectionStringBuilder(API.UserAdminServiceMethods.GetConnectionString(Nothing, WLHelperWeb.CurrentPassportID))
            Dim connectionParameters = New MsSqlConnectionParameters(connectionInfo.DataSource, connectionInfo.InitialCatalog, connectionInfo.UserID, connectionInfo.Password, MsSqlAuthorizationType.SqlServer)
            xtraReport.DataSource.ConnectionOptions.CommandTimeout = 3600
            xtraReport.DataSource.ConnectionParameters = connectionParameters
        End If
    End Sub

    Private Sub UnsetDatasourceToDevexpressReport(xtraReport As XtraReport)
        If (xtraReport.DataSource IsNot Nothing) Then
            Dim actualDS As DevExpress.DataAccess.Sql.SqlDataSource = CType(xtraReport.DataSource, DevExpress.DataAccess.Sql.SqlDataSource)
            actualDS.ConnectionParameters = Nothing
            xtraReport.DataSource = actualDS
        End If
    End Sub

    Private Function GetCustomData(report As XtraReport) As Byte()
        Dim data As Byte()

        Using ms As New MemoryStream()
            report.SaveLayoutToXml(ms)
            data = ms.GetBuffer()
        End Using

        Return data
    End Function

End Class