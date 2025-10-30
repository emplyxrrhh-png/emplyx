using Robotics.Base.DTOs;
using System.Collections.Generic;

namespace ReportGenerator.Services
{
    public interface IReportCategoryService
    {
        List<(int, int)> GetUserCategoriesList(int passportId);

        List<(int, int)> GetCategoriesIdFromReport(int reportId);

        List<roSecurityCategory> GetCategoriesFromReport(int reportId);

        //List<ReportCategory> GetCategoriesAll();

        bool InsertCategoriesToReport(int reportId, IEnumerable<(int, int)> categoryIds);

        bool DeleteCategoriesFromReport(int reportId);

        //bool DeleteCategoriesFromReport(int reportId, IEnumerable<int> categoryIds);
        //bool DeleteAllCategoriesFromReport(int reportId);
    }
}