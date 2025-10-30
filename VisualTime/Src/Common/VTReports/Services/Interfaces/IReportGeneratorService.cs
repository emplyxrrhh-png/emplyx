using Robotics.Base;
using System.Collections.Generic;

namespace ReportGenerator.Services
{
    public interface IReportGeneratorService
    {
        List<(int employeeId, byte[] reportBytes)> GenerateReportForEmployee(Report report, ReportExportExtensions extension, int userPassportId, int taskId, string language = null);

        byte[] GenerateReportForSupervisor(Report report, ReportExportExtensions extension, int userPassportId, int taskId, string language = null);

        byte[] GenerateReport(Report report, ReportExportExtensions extension, int userPassportId, int taskId, string language = null);
    }
}