using Robotics.Base;
using System.Collections.Generic;

namespace ReportGenerator.Repositories
{
    public interface IReportRepository
    {
        Report Get(int reportId);

        Report Get(string reportName);

        Report GetEmergencyReport();

        List<Report> GetByPassportCategories(int passportId);

        bool SetReportLayoutData(Report report);

        bool Insert(Report report);

        int InsertAndGetId(Report report);

        bool Update(Report report);

        bool Delete(int reportId);
    }
}