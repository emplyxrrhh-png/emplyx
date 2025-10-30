using Robotics.Base;
using System.Collections.Generic;
using VTReports.Domain;

namespace ReportGenerator.Services
{
    public interface IReportPlannedExecutionService
    {
        ReportPlannedExecution GetReportPlannedExecution(int reportPlannedExecutionId);

        List<ReportPlannedExecution> GetPlannedExecutionsFromReport(int reportId);

        List<ReportPlannedExecution> GetPlannedExecutionsFromReport(int reportId, int passportId);

        List<ReportPlannedExecution> GetPlanificationsToExecute();

        bool UpdatePlannedExecution(ReportPlannedExecution reportPlannedExecution);

        bool UpdatePlannedExecutionsFromReport(IEnumerable<ReportPlannedExecution> reportPlannedExecutions, string parametersJson, int reportId, int passportId, roReportsState oState = null, bool bAudit = false);

        bool InsertPlannedExecution(ReportPlannedExecution reportPlannedExecution);

        bool InsertPlannedExecutions(IEnumerable<ReportPlannedExecution> reportExecutions);

        bool DeleteReportPlannedExecution(int reportPlannedExecutionId);

        bool DeleteReportPlannedExecution(ReportPlannedExecution reportPlannedExecution);

        bool DeletePlannedExecutions(IEnumerable<int> reportPlannedExecutionIds);

        bool DeletePlannedExecutions(IEnumerable<ReportPlannedExecution> reportPlannedExecutions);

        bool DeleteAllPlannedExecutionFromReport(int reportId);
    }
}