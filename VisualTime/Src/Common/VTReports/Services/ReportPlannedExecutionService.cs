using ReportGenerator.Repositories;
using Robotics.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VTReports.Domain;

namespace ReportGenerator.Services
{
    public class ReportPlannedExecutionService : IReportPlannedExecutionService
    {
        #region Constants

        private const int DefaultNumberOfMaximumExecutions = 3;

        #endregion Constants

        private IPassportService passportService;
        private IVisualTimeQueryDecoderService visualTimeQueryDecoderService;
        private IReportPermissionService reportPermissionService;
        private IReportPlannedExecutionRepository reportPlannedExecutionRepository;

        #region Constructor

        public ReportPlannedExecutionService()
        {
            this.passportService = new PassportService();
            this.visualTimeQueryDecoderService = new VisualTimeQueryDecoderService();

            this.reportPlannedExecutionRepository = new ReportPlannedExecutionRepository();
            this.reportPermissionService = new ReportPermissionServiceV3();
        }

        #endregion Constructor

        public ReportPlannedExecution GetReportPlannedExecution(int reportPlannedExecutionId)
        {
            return reportPlannedExecutionRepository.GetPlannedExecution(reportPlannedExecutionId);
        }

        public List<ReportPlannedExecution> GetPlannedExecutionsFromReport(int reportId)
        {
            List<ReportPlannedExecution> reportPlannedExecutions = reportPlannedExecutionRepository.GetPlannedExecutions(reportId);
            reportPlannedExecutions.ForEach(x => x.CreatorName = passportService.GetUsernameByPassportId(x.PassportId));

            return reportPlannedExecutions;
        }

        public List<ReportPlannedExecution> GetPlannedExecutionsFromReport(int reportId, int passportId)
        {
            List<ReportPlannedExecution> reportPlannedExecutions = (IsUserAdmin(passportId)) ? reportPlannedExecutionRepository.GetPlannedExecutions(reportId) : reportPlannedExecutionRepository.GetPlannedExecutions(reportId, passportId);
            reportPlannedExecutions.ForEach(x => x.CreatorName = passportService.GetUsernameByPassportId(x.PassportId));

            return reportPlannedExecutions;
        }

        private Boolean IsUserAdmin(int passportId)
        {
            return (Robotics.Security.WLHelper.GetPermissionOverFeature(passportId, "Reports", "U") == Robotics.Base.DTOs.Permission.Admin);
        }

        public List<ReportPlannedExecution> GetPlanificationsToExecute()
        {
            return reportPlannedExecutionRepository.GetToExecute();
        }

        public bool UpdatePlannedExecutionsFromReport(IEnumerable<ReportPlannedExecution> reportPlannedExecutions, string parametersJson, int reportId, int passportId, roReportsState oState = null, bool bAudit = false)
        {
            bool result = false;

            List<ReportPlannedExecution> plannedExecutionsToDelete = null;
            List<ReportPlannedExecution> plannedExecutionsToInsert = null;
            List<ReportPlannedExecution> plannedExecutionsToUpdate = null;

            try
            {
                var plannedExecutionsComparer = new ReportPlannedExecution.ReportPlannedExecutionComparer();

                List<ReportPlannedExecution> currentPlannedExecutions = this.GetPlannedExecutionsFromReport(reportId, passportId).ToList();

                plannedExecutionsToDelete = currentPlannedExecutions.Except<ReportPlannedExecution>(reportPlannedExecutions, plannedExecutionsComparer)
                                                                    .ToList();

                //reportPlannedExecutions = reportPlannedExecutions.Where(x => x.PassportId == passportId);

                plannedExecutionsToInsert = reportPlannedExecutions.Except<ReportPlannedExecution>(currentPlannedExecutions, plannedExecutionsComparer)
                                                                   .ToList();
                plannedExecutionsToUpdate = reportPlannedExecutions.Except<ReportPlannedExecution>(plannedExecutionsToDelete, plannedExecutionsComparer)
                                                                   .Except<ReportPlannedExecution>(plannedExecutionsToInsert, plannedExecutionsComparer)
                                                                   .ToList();

                //que al guardar no sobreescriba el report id

                if (plannedExecutionsToDelete.Count() != 0)
                {
                    if (!this.reportPermissionService.GetPermissionOverReportAction(passportId, reportId, ReportPermissionTypes.DeleteSchedules)) { return false; }
                    this.DeletePlannedExecutions(plannedExecutionsToDelete);
                }

                if (plannedExecutionsToInsert.Count() != 0)
                {
                    if (!this.reportPermissionService.GetPermissionOverReportAction(passportId, reportId, ReportPermissionTypes.CreateSchedules)) { return false; }
                    plannedExecutionsToInsert.ForEach(x =>
                    {
                        x.NextExecutionDate = visualTimeQueryDecoderService.GetNextExecutionDateFromEncodedQuery(x.Scheduler, DateTime.Now);
                        if (x.LastExecutionDate == null) x.LastExecutionDate = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
                        x.RegisteredPlannedExecutionDate = DateTime.Now;
                        //   x.ParametersJson = parametersJson;
                    });

                    this.InsertPlannedExecutions(plannedExecutionsToInsert);
                }

                if (plannedExecutionsToUpdate.Count() != 0)
                {
                    if (!this.reportPermissionService.GetPermissionOverReportAction(passportId, reportId, ReportPermissionTypes.UpdateSchedules)) { return false; }
                    plannedExecutionsToUpdate.ForEach(x =>
                    {
                        x.NextExecutionDate = visualTimeQueryDecoderService.GetNextExecutionDateFromEncodedQuery(x.Scheduler, DateTime.Now);
                        //x.ParametersJson = parametersJson;
                        this.UpdatePlannedExecution(x);
                    });
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                oState.UpdateStateInfo(ex, oState.GetStateLocationInfo(this));
            }

            if (bAudit)
            {
                if (plannedExecutionsToDelete.Count > 0)
                {
                    var parametersDatatableDelete = oState.CreateAuditParameters();
                    plannedExecutionsToDelete.ForEach(x => oState.AddAuditParameter(parametersDatatableDelete, "{reportPlannedExecutions}", $"{x.Id}", "", 1));

                    oState.Audit(Robotics.VTBase.Audit.Action.aDelete,
                                 Robotics.VTBase.Audit.ObjectType.tReport,
                                 "Report Planifications List",
                                 parametersDatatableDelete,
                                 -1);
                }

                if (plannedExecutionsToInsert.Count > 0)
                {
                    var parametersDatatableInsert = oState.CreateAuditParameters();
                    plannedExecutionsToInsert.ForEach(x => oState.AddAuditParameter(parametersDatatableInsert, "{reportPlannedExecutions}", $"{x.Id}", "", 1));

                    oState.Audit(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tReport, "Report Planifications List", parametersDatatableInsert, -1);
                }

                if (plannedExecutionsToUpdate.Count > 0)
                {
                    var parametersDatatableUpdate = oState.CreateAuditParameters();
                    plannedExecutionsToUpdate.ForEach(x => oState.AddAuditParameter(parametersDatatableUpdate, "{reportPlannedExecutions}", $"{x.Id}", "", 1));

                    oState.Audit(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tReport, "Report Planifications List", parametersDatatableUpdate, -1);
                }
            }

            return result;
        }

        public bool UpdatePlannedExecution(ReportPlannedExecution reportPlannedExecution)
        {
            return reportPlannedExecutionRepository.Update(reportPlannedExecution);
        }

        public bool InsertPlannedExecutions(IEnumerable<ReportPlannedExecution> reportExecutions)
        {
            return reportPlannedExecutionRepository.Insert(reportExecutions);
        }

        public bool InsertPlannedExecution(ReportPlannedExecution reportPlannedExecution)
        {
            return reportPlannedExecutionRepository.Insert(reportPlannedExecution);
        }

        public bool DeleteReportPlannedExecution(int reportPlannedExecutionId)
        {
            return reportPlannedExecutionRepository.Delete(reportPlannedExecutionId);
        }

        public bool DeleteReportPlannedExecution(ReportPlannedExecution reportPlannedExecution)
        {
            return reportPlannedExecutionRepository.Delete(reportPlannedExecution.Id);
        }

        public bool DeletePlannedExecutions(IEnumerable<int> plannedExecutionIds)
        {
            return reportPlannedExecutionRepository.Delete(plannedExecutionIds);
        }

        public bool DeletePlannedExecutions(IEnumerable<ReportPlannedExecution> reportPlannedExecutions)
        {
            return reportPlannedExecutionRepository.Delete(reportPlannedExecutions.Select(x => x.Id));
        }

        public bool DeleteAllPlannedExecutionFromReport(int reportId)
        {
            return reportPlannedExecutionRepository.DeleteAll(reportId);
        }
    }
}