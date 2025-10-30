using ReportGenerator.Repositories;
using Robotics.Base.DTOs;
using Robotics.Security.Base;
using System.Collections.Generic;
using System.Linq;

namespace ReportGenerator.Services
{
    public class ReportCategoryService : IReportCategoryService
    {
        private IReportCategoryRepository reportCategoryRepository;

        #region Constructor

        public ReportCategoryService()
        {
            this.reportCategoryRepository = new ReportCategoryRepository();
        }

        #endregion Constructor

        public List<(int, int)> GetUserCategoriesList(int passportId)
        {
            return this.reportCategoryRepository.GetFromUser(passportId);
        }

        public List<(int, int)> GetCategoriesIdFromReport(int reportId)
        {
            return this.reportCategoryRepository.GetFromReport(reportId);
        }

        public List<roSecurityCategory> GetCategoriesFromReport(int reportId)
        {
            List<(int, int)> reportCategories = this.reportCategoryRepository.GetFromReport(reportId);

            roSecurityCategoryState bState = new roSecurityCategoryState(-1);
            List<roSecurityCategory> completeLst = roSecurityCategoryManager.LoadSecurityCategories(ref bState).ToList();
            List<roSecurityCategory> lstRet = new List<roSecurityCategory>();

            foreach ((int, int) repCaegoryId in reportCategories)
            {
                foreach (roSecurityCategory tmpCat in completeLst)
                {
                    if ((int)tmpCat.ID == repCaegoryId.Item1)
                    {
                        tmpCat.CategoryLevel = repCaegoryId.Item2;
                        lstRet.Add(tmpCat);
                        break;
                    }
                }
            }

            return lstRet;
        }

        public bool InsertCategoriesToReport(int reportId, IEnumerable<(int, int)> categoryIds)
        {
            return reportCategoryRepository.Insert(reportId, categoryIds);
        }

        public bool DeleteCategoriesFromReport(int reportId)
        {
            return reportCategoryRepository.DeleteCategories(reportId);
        }
    }
}