using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.Expressions;
using DevExpress.XtraReports.UI;
using Robotics.Base;
using Robotics.Base.VTReports.Services;
using Robotics.DataLayer;
using Robotics.VTBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ReportGenerator.Services
{
    public class ReportGeneratorService : IReportGeneratorService
    {
        private IReportService reportService;
        private IReportParameterService reportParameterService;
        private IReportTranslationService reportTranslationService;

        public ReportGeneratorService()
        {
            //TOCONTINUE ISM

            this.reportService = new ReportService();
            this.reportTranslationService = new ReportTranslationService();
            this.reportParameterService = new ReportParameterService();

            DevExpress.Utils.AzureCompatibility.Enable = true;
            CustomFunctions.Register(new ReportGenerator.Support.CF_ConvertHoursToTime());
            CustomFunctions.Register(new ReportGenerator.Support.CF_ConvertWinColorToHex());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetAccrualValueonDate());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetExpectedWorkingHoursHolidays());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetBeginHourByShift());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetAccrualValueByDefaultQuery());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetBradfordFactor());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetDirectSupervisors());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetCompanyName());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetLocalDayOfWeek());
        }

        public List<(int employeeId, byte[] reportBytes)> GenerateReportForEmployee(Report report, ReportExportExtensions extension, int passportId, int taskId, string language = null)
        {
            List<(int employeeId, byte[] reportBytes)> exportedFilesList = new List<(int, byte[])>();

            try
            {
                var memoryStream = new MemoryStream(report.LayoutXMLBinary);
                DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new VTReports.Support.roReportStorage(passportId));
                RegisterTrustedClasses();

                XtraReport xtraReport = GetXtraReportFromMemoryStream(memoryStream);

                //NECESSARY TO TRANSLATE SUBREPORTS
                this.reportTranslationService.setTranslateLanguage(language, passportId);

                ReportParameter employeesListParameter = report.ParametersList.Where(x => x.Type.Equals("Robotics.Base.employeesSelector"))
                                                                                .FirstOrDefault();
                List<employeesSelector> employeeIdsValues = ((Robotics.Base.employeesSelector[])employeesListParameter.Value).ToList();

                if (employeeIdsValues != null)
                {
                    foreach (employeesSelector employeeId in employeeIdsValues)
                    {
                        using (var exportStream = new MemoryStream())
                        {
                            try
                            {
                                var memoryStreamEmployee = new MemoryStream(report.LayoutXMLBinary);
                                DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new VTReports.Support.roReportStorage(passportId));
                                RegisterTrustedClasses();

                                XtraReport xtraReportEmployee = GetXtraReportFromMemoryStream(memoryStreamEmployee);

                                if (report.CreatorPassportId == null) reportTranslationService.TranslateXtraReport(xtraReportEmployee, report.Name, passportId);
                                if (report.ParametersList != null) reportParameterService.SetParameterValuesToXtraReportEmployee(report, xtraReportEmployee, Robotics.VTBase.roTypes.Any2Integer(employeeId.Value), taskId);

                                GenerateExportedFileBytes(xtraReportEmployee, extension, exportStream);

                                exportedFilesList.Add((int.Parse(employeeId.ToString()), exportStream.ToArray()));

                                reportParameterService.DeleteRowsFromTemporalTables(report.ParametersList, taskId);
                            }
                            catch (Exception ex)
                            {
                                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportGeneratorService::GenerateReportForEmployee:: Error while generating the report for employee.", ex);
                                throw new Exception("ReportGeneratorService::GenerateReportForEmployee:: Error while generating the report for employee.", ex);
                            }


                        }
                    }
                }

                //NECESSARY TO TRANSLATE SUBREPORTS
                this.reportTranslationService.unsetTranslateLanguage();
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportGeneratorService::GenerateReportForEmployee:: Error while processing the report for employee.", ex);
                throw new Exception("ReportGeneratorService::GenerateReportForEmployee:: Error while processing the report for employee.", ex);
            }
            
            return exportedFilesList;
            
        }

        public byte[] GenerateReportForSupervisor(Report report, ReportExportExtensions extension, int passportId, int taskId, string language = null)
        {
            try
            {
                using (var memoryStream = new MemoryStream(report.LayoutXMLBinary))
                {
                    DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new VTReports.Support.roReportStorage(passportId));
                    RegisterTrustedClasses();

                    XtraReport xtraReport = GetXtraReportFromMemoryStream(memoryStream);

                    using (xtraReport)
                    {
                        //NECESSARY TO TRANSLATE SUBREPORTS
                        this.reportTranslationService.setTranslateLanguage(language, passportId);

                        try
                        {
                            //TRANSLATE ONLY ROBOTICS' REPORTS
                            if (report.CreatorPassportId == null)
                                reportTranslationService.TranslateXtraReport(xtraReport, report.Name, passportId);

                            if (report.ParametersList != null)
                                reportParameterService.SetParameterValuesToXtraReport(report, xtraReport, taskId);

                            using (var exportStream = new MemoryStream())
                            {
                                GenerateExportedFileBytes(xtraReport, extension, exportStream);

                                //NECESSARY TO TRANSLATE SUBREPORTS
                                this.reportTranslationService.unsetTranslateLanguage();

                                reportParameterService.DeleteRowsFromTemporalTables(report.ParametersList, taskId);

                                return exportStream.ToArray();
                            }
                        }
                        catch (Exception ex)
                        {
                            roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportGeneratorService::GenerateReportForSupervisor:: Error while generating the report for supervisor.", ex);
                            throw new Exception("ReportGeneratorService::GenerateReportForSupervisor:: Error while generating the report for supervisor.", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportGeneratorService::GenerateReportForSupervisor:: Error while processing the report for supervisor.", ex);
                throw new Exception("ReportGeneratorService::GenerateReportForSupervisor:: Error while processing the report for supervisor.", ex);
            }
        }

        public byte[] GenerateReport(Report report, ReportExportExtensions extension, int userPassportId, int taskId, string language = null)
        {
            using (var memoryStream = new MemoryStream(report.LayoutXMLBinary))
            {
                DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new VTReports.Support.roReportStorage(userPassportId));
                RegisterTrustedClasses();

                XtraReport xtraReport = GetXtraReportFromMemoryStream(memoryStream);

                //NECESSARY TO TRANSLATE SUBREPORTS
                this.reportTranslationService.setTranslateLanguage(language, userPassportId);

                //TRANSLATE ONLY ROBOTICS' REPORTS
                if (report.CreatorPassportId == null) reportTranslationService.TranslateXtraReport(xtraReport, report.Name, userPassportId);
                if (report.ParametersList != null) reportParameterService.SetParameterValuesToXtraReport(report, xtraReport, taskId);

                using (var exportStream = new MemoryStream())
                {
                    GenerateExportedFileBytes(xtraReport, extension, exportStream);

                    //NECESSARY TO TRANSLATE SUBREPORTS
                    this.reportTranslationService.unsetTranslateLanguage();

                    reportParameterService.DeleteRowsFromTemporalTables(report.ParametersList, taskId);

                    return exportStream.ToArray();
                }
            }
        }

        #region Private Helper Methods

        private void RegisterTrustedClasses()
        {
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(DevExpress.Data.PivotGrid.PivotErrorValue));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.passportIdentifier));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.employeesSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.betweenYearAndMonthSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.causeIdentifier));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.causesSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.conceptGroupsSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.conceptIdentifier));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.conceptsSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.filterProfileTypesSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.filterSelectorCausesRegistroJL));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.filterSelectorConceptsRegistroJL));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.filterValuesSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.formatSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.functionCall));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.incidenceIdentifier));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.incidencesSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.shiftsSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.holidaysSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.taskIdentifier));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.tasksSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.terminalsSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.userFieldIdentifierConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.userFieldsSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.userFieldsSelectorRadioBtn));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.viewAndFormatSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.accessTypeSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.yearAndMonthSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.zonesSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.projectsVSLSelector));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.betweenYearAndMonthSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.causeIdentifierTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.CausesSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.ConceptGroupsSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.conceptIdentifierTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.ConceptSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.EmployeesSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.filterProfileTypesSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.filterSelectorCausesRegistroJLTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.filterSelectorConceptsRegistroJLTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.filterValuesSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.formatSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.FunctionCallTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.incidenceIdentifierTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.IncidencesSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.PassportIdentifierTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.ShiftsSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.HolidaysSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.TaskIdentifierTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.TasksSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.TerminalsSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.UserFieldsSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.UserFieldsSelectorRadioBtnTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.viewAndFormatSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.accessTypeSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.yearAndMonthSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.ZonesSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.projectsVSLSelectorTypeConverter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.Report));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.ReportCategory));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.ReportExecution));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.ReportParameter));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.ReportPassport));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.ReportPermissions));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.ReportPlannedDestination));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.ReportPlannedExecution));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.ReportPlannedExecution.ReportPlannedExecutionComparer));
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Robotics.Base.userFieldIdentifier));
        }

        private XtraReport GetXtraReportFromMemoryStream(MemoryStream memoryStream)
        {
            try
            {
                SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(AccessHelper.GetConectionString());

                using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
                {
                    connection.Open();

                    string serverName = connectionStringBuilder.DataSource;
                    string databaseName = connectionStringBuilder.InitialCatalog;
                    string userName = connectionStringBuilder.UserID;
                    string password = connectionStringBuilder.Password;

                    var connectionParameters = new MsSqlConnectionParameters(serverName, databaseName, userName, password, MsSqlAuthorizationType.SqlServer);

                    XtraReport xtraReport = XtraReport.FromStream(memoryStream);

                    if (xtraReport.DataSource != null)
                    {
                        var datasource = xtraReport.DataSource as SqlDataSource;
                        datasource.ConnectionParameters = connectionParameters;
                        datasource.ConnectionOptions.CommandTimeout = 3600;
                    }

                    return xtraReport;
                }
            }
            catch (SqlException sqlEx)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportGeneratorService::GetXtraReportFromMemoryStream:: SQL error while getting the XtraReport from memory stream.", sqlEx);
                throw new Exception("ReportGeneratorService::GetXtraReportFromMemoryStream:: SQL error while getting the XtraReport from memory stream.", sqlEx);
            }
            catch (InvalidOperationException invOpEx)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportGeneratorService::GetXtraReportFromMemoryStream:: Invalid operation error while getting the XtraReport from memory stream.", invOpEx);
                throw new Exception("ReportGeneratorService::GetXtraReportFromMemoryStream:: Invalid operation error while getting the XtraReport from memory stream.", invOpEx);
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportGeneratorService::GetXtraReportFromMemoryStream:: Error while getting the XtraReport from memory stream.", ex);
                throw new Exception("ReportGeneratorService::GetXtraReportFromMemoryStream:: Error while getting the XtraReport from memory stream.", ex);
            }
        }


        private void GenerateExportedFileBytes(XtraReport report, ReportExportExtensions extension, MemoryStream exportStream)
        {
            try
            {
                roTrace.get_GetInstance().InitTraceEvent();
                switch (extension)
                {
                    case ReportExportExtensions.pdf:
                        new PdfStreamingExporter(report, true).Export(exportStream);
                        break;

                    case ReportExportExtensions.xlsx:
                        report.ExportToXlsx(exportStream);
                        break;

                    case ReportExportExtensions.xls:
                        report.ExportToXls(exportStream);
                        break;

                    case ReportExportExtensions.csv:
                        report.ExportToCsv(exportStream);
                        break;

                    case ReportExportExtensions.docx:
                        report.ExportToDocx(exportStream);
                        break;

                    case ReportExportExtensions.text:
                        report.ExportToText(exportStream);
                        break;

                    case ReportExportExtensions.mail:
                        new PdfStreamingExporter(report, true).Export(exportStream);   //OJOOOOOOOOOOOOOOOOOOOOOOOOOOUUUUUUUUUUUUUU
                        break;

                    case ReportExportExtensions.image:
                        report.ExportToImage(exportStream);
                        break;

                    case ReportExportExtensions.mht:
                        report.ExportToMht(exportStream);
                        break;

                    case ReportExportExtensions.rtf:
                        report.ExportToRtf(exportStream);
                        break;
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "ReportGeneratorService::GenerateExportedFileBytes:: Error while generating the exported file bytes.", ex);
                throw new Exception("ReportGeneratorService::GenerateExportedFileBytes:: Error while generating the exported file bytes.", ex);
            }
            finally
            {
                roTrace.get_GetInstance().AddTraceEvent($"Devexpress report {report.Name} generated");
            }
        }


        #endregion Private Helper Methods
    }
}