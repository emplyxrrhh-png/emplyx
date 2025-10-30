using ReportGenerator.Repositories;
using Robotics.Base;
using Robotics.Base.DTOs;
using Robotics.VTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VTReports.Domain;

namespace ReportGenerator.Services
{
    public class ReportExecutionService : IReportExecutionService
    {
        #region Constants

        private const int DefaultNumberOfMaximumExecutions = 3;

        #endregion Constants

        private IBlobRepository blobRepository;
        private IReportExecutionRepository reportExecutionRepository;
        private IReportPermissionService reportPermissionService;

        #region Constructor

        public ReportExecutionService()
        {
            this.reportExecutionRepository = new ReportExecutionRepository();
            this.reportPermissionService = new ReportPermissionServiceV3();
        }

        #endregion Constructor

        public ReportExecution GetExecution(Guid executionId)
        {
            return reportExecutionRepository.Get(executionId);
        }

        public List<ReportExecution> GetExecutionsFromReport(int reportId)
        {
            List<ReportExecution> auxiliarExecutionsList = (reportExecutionRepository.Get(reportId) ?? new List<ReportExecution>());
            List<ReportExecution> executionstoRemove = auxiliarExecutionsList.Reverse<ReportExecution>().Take(auxiliarExecutionsList.Count() - DefaultNumberOfMaximumExecutions).ToList();

            try
            {
                this.blobRepository = new BlobRepository(Robotics.Azure.RoAzureSupport.GetCompanyName());
                executionstoRemove.ForEach(x => this.blobRepository.DeleteBlob(x));

                if (executionstoRemove.Count > 0) reportExecutionRepository.Delete(executionstoRemove.Select(x => x.Guid));
            }
            catch (Exception) {
                // do nothing
            }

            return auxiliarExecutionsList.Take(DefaultNumberOfMaximumExecutions).ToList();
        }

        public List<ReportExecution> GetExecutionsFromReport(int reportId, int passportId)
        {
            List<ReportExecution> auxiliarExecutionsList = (reportExecutionRepository.Get(reportId, passportId) ?? new List<ReportExecution>());
            auxiliarExecutionsList = auxiliarExecutionsList.OrderByDescending(x => x.ExecutionDate).ToList();

            List<ReportExecution> executionstoRemove = auxiliarExecutionsList.Reverse<ReportExecution>().Take(auxiliarExecutionsList.Count() - (DefaultNumberOfMaximumExecutions + 1)).ToList();

            try
            {
                this.blobRepository = new BlobRepository(Robotics.Azure.RoAzureSupport.GetCompanyName());
                executionstoRemove.ForEach(x => this.blobRepository.DeleteBlob(x));

                if (executionstoRemove.Count > 0) reportExecutionRepository.Delete(executionstoRemove.Select(x => x.Guid));
            }
            catch (Exception) {
                // do nothing
            }

            return auxiliarExecutionsList.Take(DefaultNumberOfMaximumExecutions).ToList();
        }

        public ReportExecution GetLastExecutionFromReport(int reportId, int passportId)
        {
            return reportExecutionRepository.Get(reportId, passportId).OrderByDescending(x => x.ExecutionDate).FirstOrDefault();
        }

        public int GetNumberOfMaximumExecutions()
        {
            var oAdvState = new Robotics.Base.VTBusiness.Common.AdvancedParameter.roAdvancedParameterState(-1);
            int maximumExecutions = 0;
            bool conversionSucceeded = Int32.TryParse(new Robotics.Base.VTBusiness.Common.AdvancedParameter.roAdvancedParameter("VTLive.Reports.NumberOfMaximumExecutions", oAdvState).Value, out maximumExecutions);

            return (conversionSucceeded ? maximumExecutions : DefaultNumberOfMaximumExecutions);
        }

        public bool UpdateExecutionsFromReport(IEnumerable<ReportExecution> reportExecutions, int reportId, int passportId, roReportsState oState = null, bool bAudit = false)
        {
            bool result = false;

            List<Guid> currentExecutions = null;
            List<Guid> executionsToDelete = null;

            try
            {
                currentExecutions = this.GetExecutionsFromReport(reportId, passportId).Select(x => x.Guid).ToList();
                executionsToDelete = currentExecutions.Except(reportExecutions.Select(x => x.Guid)).ToList();

                if (executionsToDelete.Count() != 0)
                {
                    if (!this.reportPermissionService.GetPermissionOverReportAction(passportId, reportId, ReportPermissionTypes.DeleteExecutions)) { return false; }
                    this.DeleteExecutions(executionsToDelete);
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                oState.UpdateStateInfo(ex, oState.GetStateLocationInfo(this));
            }

            if (bAudit && executionsToDelete.Count() != 0)
            {
                var parametersDatatable = oState.CreateAuditParameters();
                executionsToDelete.ForEach(x => oState.AddAuditParameter(parametersDatatable, "{reportExecutionsList}", $"GUID: {x}", "", 1));

                oState.Audit(Robotics.VTBase.Audit.Action.aDelete,
                             Robotics.VTBase.Audit.ObjectType.tReport,
                             "Report Executions List",
                             parametersDatatable,
                             -1);
            }

            return result;
        }

        public Guid InsertExecution(ReportExecution execution)
        {
            return reportExecutionRepository.Insert(execution);
        }

        public bool DeleteExecution(Guid executionId)
        {
            return reportExecutionRepository.Delete(executionId);
        }

        public bool DeleteExecutions(IEnumerable<Guid> executionIds)
        {
            return reportExecutionRepository.Delete(executionIds);
        }

        public bool DeleteAllExecutionsFromReport(int reportId)
        {
            return reportExecutionRepository.DeleteAll(reportId);
        }
    }
}