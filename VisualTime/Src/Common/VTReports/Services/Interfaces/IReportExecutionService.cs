using Robotics.Base;
using System;
using System.Collections.Generic;
using VTReports.Domain;

namespace ReportGenerator.Services
{
    public interface IReportExecutionService
    {
        ReportExecution GetExecution(Guid executionId);

        List<ReportExecution> GetExecutionsFromReport(int reportId);

        List<ReportExecution> GetExecutionsFromReport(int reportId, int passportId);

        ReportExecution GetLastExecutionFromReport(int reportId, int passportId);

        int GetNumberOfMaximumExecutions();

        Guid InsertExecution(ReportExecution execution);

        bool UpdateExecutionsFromReport(IEnumerable<ReportExecution> reportExecutions, int reportId, int passportId, roReportsState oState = null, bool bAudit = false);

        bool DeleteExecution(Guid executionId);

        bool DeleteExecutions(IEnumerable<Guid> executionIds);

        bool DeleteAllExecutionsFromReport(int reportId);
    }
}