using System.Collections.Generic;

namespace ReportGenerator.Repositories
{
    public interface IReportCategoryRepository
    {
        List<(int, int)> GetFromUser(int passportId);

        List<(int, int)> GetFromReport(int reportId);

        bool Insert(int reportId, IEnumerable<(int, int)> categoryIds);

        bool DeleteCategories(int reportId);
    }
}