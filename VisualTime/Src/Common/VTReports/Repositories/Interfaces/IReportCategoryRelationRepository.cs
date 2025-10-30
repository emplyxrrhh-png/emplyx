using ReportGenerator.Domain;
using Robotics.Base;
using System;
using System.Collections.Generic;
using System.Data;

namespace ReportGenerator.Repositories
{
    public interface IReportCategoryRelationRepository
    {
        List<int> GetCategoryIds(int reportId);

        bool Insert(int reportId, IEnumerable<int> categoryIds);

        bool Delete(int reportId, IEnumerable<int> categoryIds);
        bool DeleteAll(int reportId);
    }
}