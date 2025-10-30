using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.XtraReports.Expressions;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json;
using Robotics.Base;
using Robotics.Base.DTOs;
using Robotics.VTBase;
using Robotics.Web.Base;
using Robotics.Web.Base.API;
using Robotics.Web.VTLiveMvC.Support;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace Robotics.Web.VTLiveMvC.Controllers
{
    public class ReportsController
    {
        public object OpenReportDesigner()
        {
            XtraReport report = new XtraReport();
            report.Extensions[SerializationService.Guid] = CustomDataSerializer.Name;

            report.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
                new DevExpress.XtraReports.Parameters.Parameter{ Name = "PassportId",
                                                                 Description = "The user's unique identifier",
                                                                 Type = typeof(passportIdentifier)}
            });

            var connectionInfo = new SqlConnectionStringBuilder(Base.API.UserAdminServiceMethods.GetConnectionString(null, WLHelperWeb.CurrentPassportID));
            var connectionParameters = new MsSqlConnectionParameters(connectionInfo.DataSource, connectionInfo.InitialCatalog, connectionInfo.UserID, connectionInfo.Password, MsSqlAuthorizationType.SqlServer);

            DevExpress.DataAccess.Sql.SqlDataSource actualDS = new DevExpress.DataAccess.Sql.SqlDataSource(connectionParameters);
            actualDS.Name = "VisualTime";
            report.DataSource = actualDS;

            return report;
        }

        public object OpenReportDesigner(int Id)
        {
            CustomFunctions.Register(new ReportGenerator.Support.CF_ConvertHoursToTime());
            CustomFunctions.Register(new ReportGenerator.Support.CF_ConvertWinColorToHex());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetAccrualValueonDate());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetExpectedWorkingHoursHolidays());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetBeginHourByShift());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetAccrualValueByDefaultQuery());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetBradfordFactor());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetCompanyName());
            CustomFunctions.Register(new ReportGenerator.Support.CF_GetLocalDayOfWeek());

            XtraReport report;
            Report oReportLayout = ReportServiceMethods.GetReportById(Id, ReportPermissionTypes.Update, null);
            using (var layoutStream = new MemoryStream(oReportLayout.LayoutXMLBinary))
            {
                report = XtraReport.FromStream(layoutStream);
                report.Extensions[SerializationService.Guid] = CustomDataSerializer.Name;

                var connectionInfo = new SqlConnectionStringBuilder(Base.API.UserAdminServiceMethods.GetConnectionString(null, WLHelperWeb.CurrentPassportID));
                var connectionParameters = new MsSqlConnectionParameters(connectionInfo.DataSource, connectionInfo.InitialCatalog, connectionInfo.UserID, connectionInfo.Password, MsSqlAuthorizationType.SqlServer);

                if (report.DataSource != null)
                {
                    DevExpress.DataAccess.Sql.SqlDataSource actualDS = (DevExpress.DataAccess.Sql.SqlDataSource)report.DataSource;
                    actualDS.ConnectionParameters = connectionParameters;
                    report.DataSource = actualDS;
                }
            }

            return report;
        }

        public List<Report> GetReportsDataView()
        {
            var reportsList = ReportServiceMethods.GetReportsSimplified(null);
            return reportsList;
        }

        public Byte[] GetExportDataAndExtension(Guid executionId)
        {
            ReportExecution oExecution = ReportServiceMethods.GetExecutionStatus(executionId, null);

            if (ReportServiceMethods.GetReportById(oExecution.ReportID, ReportPermissionTypes.Read, null) != null)
            {
                (Byte[], string) auxiliarExportObject = ReportServiceMethods.GetExecutionAssocietedExportFile(executionId, null);
                var blobAndExtension = new { exportData = auxiliarExportObject.Item1, extension = auxiliarExportObject.Item2 };
                return blobAndExtension.exportData;
            }
            else
            {
                return new byte[0];
            }
        }

        public int GetExecutionStatus(Guid executionId)
        {
            ReportExecution oExecution = ReportServiceMethods.GetExecutionStatus(executionId, null);

            if (ReportServiceMethods.GetReportById(oExecution.ReportID, ReportPermissionTypes.Read, null) != null)
            {
                return oExecution.Status;
            }
            else
            {
                return 3;
            }
        }

        public String GetReportsPageConfiguration()
        {
            return ReportServiceMethods.GetReportsPageConfiguration(null);
        }

        public string GetGenericParameterSelectorOptions(string parameterType, bool isEmergencyReport)
        {
            return roJSONHelper.SerializeNewtonSoft(new Models.ReportParameters().GetGenericParameterSelectorOptions(parameterType, isEmergencyReport));
        }

        public List<ReportCategory> GetReportCategories()
        {
            return new List<ReportCategory>();
        }

        public Report GetReportById(int idReport, ReportPermissionTypes action)
        {
            Report report = ReportServiceMethods.GetReportById(idReport, action, null);
            if (report != null)
            {
                report.ParametersList = new Models.ReportParameters().GetReportParameters(report.ParametersJson ?? String.Empty);
                report.LayoutXMLBinary = null;
            }
            return report;
        }

        public roPassport[] GetUsers()
        {
            return SecurityV3ServiceMethods.GetAllAvailableSupervisorsList(null, true);
        }

        public bool SaveReportInfo(string reportJson, string flag)
        {
            Report report = JsonConvert.DeserializeObject<Report>(reportJson);

            bool hasPermissionOverReport = ReportServiceMethods.GetReportById(report.Id, ReportPermissionTypes.Update, null) != null;
            if (hasPermissionOverReport)
            {
                switch (flag)
                {
                    case "report":
                        return ReportServiceMethods.UpdateReport(null, report, true);

                    case "planifications":
                        return ReportServiceMethods.UpdateReportPlannedExecutions(report.PlannedExecutionsList, JsonConvert.SerializeObject(report.ParametersList), report.Id, null, report, true);

                    case "executions":
                        return ReportServiceMethods.UpdateReportExecutions(report.ExecutionsList, report.Id, null, report, true);

                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool DeleteReportLayout(int reportId)
        {
            bool hasPermissionOverReport = ReportServiceMethods.GetReportById(reportId, ReportPermissionTypes.Delete, null) != null;
            if (hasPermissionOverReport) return ReportServiceMethods.DeleteReport(null, reportId, false);
            else return false;
        }

        public bool CopyReport(int reportId, string newReportName)
        {
            bool hasPermissionOverReport = ReportServiceMethods.GetReportById(reportId, ReportPermissionTypes.Create, null) != null;
            if (hasPermissionOverReport) return ReportServiceMethods.CopyReport(reportId, newReportName, WLHelperWeb.CurrentPassportID, null, false);
            else return false;
        }

        public bool UpdateReportCategories(int reportId, List<(int, int)> reportCategories)
        {
            bool hasPermissionOverReport = ReportServiceMethods.GetReportById(reportId, ReportPermissionTypes.Update, null) != null;
            if (hasPermissionOverReport) return ReportServiceMethods.UpdateReportCategories(reportId, reportCategories, WLHelperWeb.CurrentPassportID, null, false);
            else return false;
        }

        public int TriggerExecution(int reportId, string lastParameters, string viewFields)
        {
            bool hasPermissionOverReport = ReportServiceMethods.GetReportById(reportId, ReportPermissionTypes.Update, null) != null;
            if (hasPermissionOverReport)
            {
                if (ReportServiceMethods.SaveReportLastParameters(lastParameters, reportId, WLHelperWeb.CurrentPassportID, null, false))
                {
                    return LiveTasksServiceMethods.CreateReportTaskDX(false, reportId, lastParameters, viewFields, true, null);
                }
            }

            return -1;
        }
    }
}