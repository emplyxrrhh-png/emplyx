using DevExpress.XtraReports.UI;
using ReportGenerator.Repositories;
using ReportGenerator.Services;
using ReportGenerator.TaskManager;
using Robotics.Base.DTOs;
using Robotics.Security;
using Robotics.VTBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VTReports.Domain;

namespace Robotics.Base.VTReports.Services
{
    public class ReportService : IReportService
    {
        private IReportRepository reportRepository;
        private IPassportService passportService;
        private IReportExecutionService reportExecutionService;
        private IReportParameterService reportParameterService;
        private IReportPermissionService reportPermissionService;
        private IReportCategoryService reportCategoryService;
        private IReportPlannedExecutionService reportPlannedExecutionService;

        #region Constructor

        public ReportService()
        {
            //TOCONTINUE ISM

            this.reportRepository = new ReportRepository();
            this.passportService = new PassportService();
            this.reportCategoryService = new ReportCategoryService();
            this.reportParameterService = new ReportParameterService();
            this.reportPlannedExecutionService = new ReportPlannedExecutionService();
            this.reportExecutionService = new ReportExecutionService();
            this.reportPermissionService = new ReportPermissionServiceV3();
        }

        public ReportService(IReportRepository reportRepository)
        {
            this.reportRepository = reportRepository;
        }

        #endregion Constructor

        #region Methods

        public Report GetReportById(int reportId, int passportId, roReportsState oState = null, bool bAudit = false)
        {
            Report report = null;

            try
            {
                if (!this.reportPermissionService.GetPermissionOverReportAction(passportId, reportId, ReportPermissionTypes.Read)) { return report; }

                report = reportRepository.Get(reportId);

                report.CreatorName = passportService.GetUsernameByPassportId(report.CreatorPassportId ?? 0);
                report.Permissions = reportPermissionService.GetPermissionsOverReport(report, passportId);
                report.LastParameters = reportParameterService.GetLastParametersFromReport(reportId, passportId);
                report.ExecutionsList = reportExecutionService.GetExecutionsFromReport(report.Id, passportId);
                report.LastExecution = reportExecutionService.GetLastExecutionFromReport(report.Id, passportId);
                report.PlannedExecutionsList = (IsUserAdmin(passportId)) ? reportPlannedExecutionService.GetPlannedExecutionsFromReport(report.Id) : reportPlannedExecutionService.GetPlannedExecutionsFromReport(report.Id, passportId);

                report.CategoriesList = this.reportCategoryService.GetCategoriesFromReport(reportId);
            }
            catch (Exception ex)
            {
                oState.UpdateStateInfo(ex, oState.GetStateLocationInfo(this));
            }

            if (bAudit)
            {
                oState.Audit(VTBase.Audit.Action.aSelect,
                             VTBase.Audit.ObjectType.tReport,
                             oState.GetAuditObjectName(reportId, report.Name, null),
                             oState.CreateAuditParameters(),
                             -1);
            }

            return report;
        }

        private Boolean IsUserAdmin(int passportId)
        {
            return (Robotics.Security.WLHelper.GetPermissionOverFeature(passportId, "Reports", "U") == Robotics.Base.DTOs.Permission.Admin);
        }

        public Report GetReportByName(string reportName, int passportId, ReportPermissionTypes action = ReportPermissionTypes.Update, roReportsState oState = null, bool bAudit = false)
        {
            Report report = null;

            try
            {
                report = reportRepository.Get(reportName);

                if (!this.reportPermissionService.GetPermissionOverReportAction(passportId, report, action)) { report = null; }
            }
            catch (Exception ex)
            {
                oState.UpdateStateInfo(ex, oState.GetStateLocationInfo(this));
            }

            if (bAudit)
            {
                oState.Audit(VTBase.Audit.Action.aSelect,
                             VTBase.Audit.ObjectType.tReport,
                             oState.GetAuditObjectName(report?.Id, reportName, (report != null)),
                             oState.CreateAuditParameters(),
                             -1);
            }

            return report;
        }

        public Report GetEmergencyReport(int passportId, roReportsState oState = null, bool bAudit = false)
        {
            Report report = null;

            try
            {
                report = reportRepository.GetEmergencyReport();

                report.CreatorName = passportService.GetUsernameByPassportId(report.CreatorPassportId ?? 0);
                report.Permissions = reportPermissionService.GetPermissionsOverReport(report, passportId);
                report.LastParameters = reportParameterService.GetLastParametersFromReport(report.Id, passportId);
                report.ExecutionsList = reportExecutionService.GetExecutionsFromReport(report.Id);
                report.LastExecution = reportExecutionService.GetLastExecutionFromReport(report.Id, passportId);
                report.PlannedExecutionsList = reportPlannedExecutionService.GetPlannedExecutionsFromReport(report.Id);
            }
            catch (Exception ex)
            {
                oState.UpdateStateInfo(ex, oState.GetStateLocationInfo(this));
            }

            if (bAudit)
            {
                oState.Audit(VTBase.Audit.Action.aSelect,
                             VTBase.Audit.ObjectType.tReport,
                             oState.GetAuditObjectName(report?.Id, report.Name, (report != null)),
                             oState.CreateAuditParameters(),
                             -1);
            }

            return report.PlannedExecutionsList.Count > 0 ? report : null;
        }

        public Report GetDevexpressCompatibleReportById(int reportId, bool isSupervisorReport, string reportParameters, int passportId, int taskId)
        {
            Report report = reportRepository.Get(reportId);

            report.ParametersJson = reportParameters;
            report.ParametersList = reportParameterService.GetDevexpressCompatibleReportParameters(report.ParametersJson ?? String.Empty, passportId, taskId);

            return report;
        }

        public List<Report> GetReportsByPassportIdSimplified(int passportId, roReportsState oState = null, bool bAudit = false)
        {
            List<Report> reportsList = new List<Report>();

            try
            {
                if (!this.reportPermissionService.GetGeneralReportPermission(passportId, ReportPermissionTypes.Read)) { return reportsList; }

                List<(int, int)> oUserCategories = null;
                List<Report> tmpList = new List<Report>();

                tmpList = reportRepository.GetByPassportCategories(passportId);
                oUserCategories = this.reportCategoryService.GetUserCategoriesList(passportId);

                foreach (Report report in tmpList)
                {
                    bool bInsert = this.reportPermissionService.GetPermissionOverReportAction(passportId, report, ReportPermissionTypes.Read);
                    report.LayoutXMLBinary = null;
                    report.LastExecution = reportExecutionService.GetLastExecutionFromReport(report.Id, passportId);

                    if (bInsert)
                    {
                        report.CategoriesDescription = string.Empty;
                        if (report.CategoriesList != null)
                        {
                            foreach (roSecurityCategory oCat in report.CategoriesList)
                            {
                                report.CategoriesDescription = "cat:" + oCat.Description + ";";
                            }
                        }

                        reportsList.Add(report);
                    }
                }
            }
            catch (Exception ex)
            {
                oState.UpdateStateInfo(ex, oState.GetStateLocationInfo(this));
            }

            if (bAudit)
            {
                var parametersDatatable = oState.CreateAuditParameters();
                reportsList.ForEach(x => oState.AddAuditParameter(parametersDatatable, "{reportsList}", $"{x.Id}: {x.Name}", "", 1));

                oState.Audit(VTBase.Audit.Action.aMultiSelect,
                             VTBase.Audit.ObjectType.tReport,
                             "Reports List",
                             parametersDatatable,
                             -1);
            }

            return reportsList;
        }

        public int? GetReportCreatorPassportId(int reportId)
        {
            return reportRepository.Get(reportId).CreatorPassportId;
        }

        public bool InsertReport(Report report, roReportsState oState, bool bAudit)
        {
            bool result = false;

            try
            {
                result = reportRepository.Insert(report);

                if (bAudit)
                {
                    oState.Audit(VTBase.Audit.Action.aInsert,
                                 VTBase.Audit.ObjectType.tReport,
                                 oState.GetAuditObjectName(null, report.Name, result),
                                 oState.CreateAuditParameters(),
                                 -1);
                }
            }
            catch (Exception ex)
            {
                oState.UpdateStateInfo(ex, oState.GetStateLocationInfo(this));
            }

            return result;
        }

        public bool UpdateReport(Report report, roReportsState oState, bool bAudit)
        {
            bool result = false;

            try
            {
                result = reportRepository.Update(report);
                if (bAudit)
                {
                    oState.Audit(VTBase.Audit.Action.aUpdate, VTBase.Audit.ObjectType.tReport, oState.GetAuditObjectName(report.Id, report.Name, result), oState.CreateAuditParameters(), -1);
                }
            }
            catch (Exception ex)
            {
                oState.UpdateStateInfo(ex, oState.GetStateLocationInfo(this));
            }

            return result;
        }

        public bool DeleteReport(int reportId, int passportId, roReportsState oState = null, bool bAudit = false)
        {
            bool result = false;

            try
            {
                if (!this.reportPermissionService.GetPermissionOverReportAction(passportId, reportId, ReportPermissionTypes.Update)) { return false; }

                result = reportParameterService.DeleteAllLastParameters(reportId) &&
                         reportExecutionService.DeleteAllExecutionsFromReport(reportId) &&
                         reportPlannedExecutionService.DeleteAllPlannedExecutionFromReport(reportId) &&
                         reportCategoryService.DeleteCategoriesFromReport(reportId) &&
                         reportRepository.Delete(reportId);
            }
            catch (Exception ex)
            {
                oState.UpdateStateInfo(ex, oState.GetStateLocationInfo(this));
            }

            if (bAudit)
            {
                oState.Audit(VTBase.Audit.Action.aDelete, VTBase.Audit.ObjectType.tReport, oState.GetAuditObjectName(reportId, null, result), oState.CreateAuditParameters(), -1);
            }

            return result;
        }

        public bool CopyReport(int reportId, string newReportName, int passportId, roReportsState oState = null, bool bAudit = false)
        {
            bool result = false;

            try
            {
                if (!this.reportPermissionService.GetPermissionOverReportAction(passportId, reportId, ReportPermissionTypes.CopyReport)) { return false; }

                result = this.CopyReportRecursive(reportId, newReportName, passportId);
            }
            catch (Exception ex)
            {
                result = false;

                oState.UpdateStateInfo(ex, oState.GetStateLocationInfo(this));
            }

            if (bAudit)
            {
                oState.Audit(VTBase.Audit.Action.aInsert, VTBase.Audit.ObjectType.tReport, oState.GetAuditObjectName(reportId, newReportName, result), oState.CreateAuditParameters(), -1);
            }

            return result;
        }

        public bool UpdateReportCategories(int reportId, List<(int, int)> reportCategories, int passportId, roReportsState oState = null, bool bAudit = false)
        {
            bool result = false;

            try
            {
                if (!this.reportPermissionService.GetPermissionOverReportAction(passportId, reportId, ReportPermissionTypes.CopyReport)) { return false; }

                result = this.UpdateReportCategories(reportId, reportCategories);
            }
            catch (Exception ex)
            {
                result = false;
                oState.UpdateStateInfo(ex, oState.GetStateLocationInfo(this));
            }

            if (bAudit)
            {
                oState.Audit(VTBase.Audit.Action.aUpdate, VTBase.Audit.ObjectType.tReport, oState.GetAuditObjectName(reportId, "", result), oState.CreateAuditParameters(), -1);
            }

            return result;
        }

        public bool ExecuteEmergencyReport(int passportId, roReportsState oState = null, bool bAudit = false, string strReports = null)
        {
            bool result = false;
            Report report = null;

            try
            {
                report = reportRepository.GetEmergencyReport();

                report.PlannedExecutionsList = reportPlannedExecutionService.GetPlannedExecutionsFromReport(report.Id);
                foreach (ReportPlannedExecution plannedExecution in report.PlannedExecutionsList)
                {
                    //ejecutamos los informes planificados
                    if (strReports == null || strReports.Split(',').Contains(plannedExecution.Id.ToString()))
                    {
                        plannedExecution.NextExecutionDate = DateTime.Now.Subtract(new TimeSpan(0, 1, 0));
                        plannedExecution.PassportId = passportId;
                        this.reportPlannedExecutionService.UpdatePlannedExecution(plannedExecution);
                    }
                }

                result = ReportTaskExecution.ExecuteScheduledTasks().Result;
            }
            catch (Exception ex)
            {
                result = false;
                oState.UpdateStateInfo(ex, oState.GetStateLocationInfo(this));
            }

            if (bAudit)
            {
                oState.Audit(VTBase.Audit.Action.aInsert, VTBase.Audit.ObjectType.tReport, oState.GetAuditObjectName(report?.Id, report?.Name, result), oState.CreateAuditParameters(), -1);
            }

            return result;
        }

        #region Private Helper Methods

        private bool CopyReportRecursive(int reportId, string newReportName, int passportId)
        {
            Report reportToCopy = reportRepository.Get(reportId);
            List<(int, int)> reportCategories = reportCategoryService.GetCategoriesIdFromReport(reportId);
            XtraReport xtraReport = XtraReport.FromStream(new MemoryStream(reportToCopy.LayoutXMLBinary));
            xtraReport.Name = newReportName.Replace(" ", "_");
            xtraReport.DisplayName = newReportName;

            reportToCopy.CreatorPassportId = passportId;
            reportToCopy.Name = newReportName;
            reportToCopy.CreationDate = DateTime.Now;
            reportToCopy.IsEmergencyReport = false;

            //SUBREPORT COPY LOGIC
            ///////////////////////////////////////////////////
            List<XRSubreport> subreportsList = xtraReport.AllControls<XRSubreport>().ToList();

            foreach (XRSubreport xrSubreport in subreportsList)
            {
                Report copySubreport = this.GetReportByName(xrSubreport.ReportSourceUrl, passportId, ReportPermissionTypes.CopyReport);
                string newSubreportName = String.Concat("Subreport ", newReportName, "_", (xrSubreport.ReportSourceUrl).Replace("Subreport ", "").Replace("Subreport", ""));

                if (this.CopyReportRecursive(copySubreport.Id, newSubreportName, passportId))
                {
                    xrSubreport.ReportSourceUrl = newSubreportName;
                }
            }
            ///////////////////////////////////////////////////

            MemoryStream ms = new MemoryStream();
            xtraReport.SaveLayoutToXml(ms);
            reportToCopy.LayoutXMLBinary = ms.GetBuffer();

            int newReportId = reportRepository.InsertAndGetId(reportToCopy);
            return newReportId != 0 && this.reportCategoryService.InsertCategoriesToReport(newReportId, reportCategories);
        }

        private bool UpdateReportCategories(int reportId, List<(int, int)> repCategories)
        {
            return this.reportCategoryService.DeleteCategoriesFromReport(reportId) && this.reportCategoryService.InsertCategoriesToReport(reportId, repCategories);
        }

        #endregion Private Helper Methods

        #endregion Methods
    }
}