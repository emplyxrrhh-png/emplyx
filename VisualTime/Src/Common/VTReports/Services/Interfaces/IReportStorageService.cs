using Robotics.Base;
using System;

namespace ReportGenerator.Services
{
    public interface IReportStorageService
    {
        Guid Save(ReportExecution reportFile);

        string getFileLink(ReportExecution reportFile);
    }
}