using Robotics.Base;
using System;
using System.Collections.Generic;

namespace ReportGenerator.Repositories
{
    public interface IReportExecutionRepository
    {
        ReportExecution Get(Guid executionId);

        List<ReportExecution> Get(int reportId);

        List<ReportExecution> Get(int reportId, int passportId);

        List<ReportExecution> Get(IEnumerable<Guid> executionIds);

        Guid Insert(ReportExecution reportExecution);

        bool Update(ReportExecution reportExecution);

        bool Delete(Guid executionId);

        bool Delete(IEnumerable<Guid> executionIds);

        bool DeleteAll(int reportId);
    }
}