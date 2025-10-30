using Robotics.Base;
using System.Collections.Generic;

namespace ReportGenerator.Repositories
{
    public interface IReportPlannedExecutionRepository
    {
        ReportPlannedExecution GetPlannedExecution(int plannedExecutionId);

        List<ReportPlannedExecution> GetPlannedExecutions(int reportId);

        List<ReportPlannedExecution> GetPlannedExecutions(int reportId, int passportId);

        List<ReportPlannedExecution> GetToExecute();

        bool Insert(ReportPlannedExecution reportPlannedExecution);

        bool Insert(IEnumerable<ReportPlannedExecution> reportPlannedExecutions);

        bool Update(ReportPlannedExecution reportPlannedExecution);

        bool Delete(int plannedExecutionId);

        bool Delete(IEnumerable<int> plannedExecutionIds);

        bool DeleteAll(int reportId);
    }
}