using Robotics.DataLayer;
using Robotics.VTBase;
using System.Collections.Generic;
using System.Data;

namespace ReportGenerator.Repositories
{
    public class ReportCategoryRepository : IReportCategoryRepository
    {
        #region Constructors

        public ReportCategoryRepository()
        {
        }

        #endregion Constructors

        #region Methods

        public List<(int, int)> GetFromUser(int passportId)
        {
            List<(int, int)> categories = new List<(int, int)>();

            string sql = "@SELECT# DISTINCT IDCategory,LevelOfAuthority FROM sysroPassports_Categories WHERE IDPassport = @passportId";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@passportId", CommandParameter.ParameterType.tInt, passportId));

            DataTable dataTable;
            dataTable = AccessHelper.CreateDataTable(sql, oParams);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    categories.Add((int.Parse(row["IDCategory"].ToString()), int.Parse(row["LevelOfAuthority"].ToString())));
                }
            }

            return categories;
        }

        public List<(int, int)> GetFromReport(int reportId)
        {
            List<(int, int)> categories = new List<(int, int)>();

            string sql = "@SELECT# DISTINCT CategoryId,CategoryLevel FROM ReportLayoutCategories WHERE ReportLayoutId = @reportId";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@reportId", CommandParameter.ParameterType.tInt, reportId));

            DataTable dataTable;
            dataTable = AccessHelper.CreateDataTable(sql, oParams);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    categories.Add((roTypes.Any2Integer(row["CategoryId"]), roTypes.Any2Integer(row["CategoryLevel"])));
                }
            }
            return categories;
        }

        public bool Insert(int reportId, IEnumerable<(int, int)> categoryIds)
        {
            string sql = "@INSERT# " +
                         "INTO ReportLayoutCategories " +
                         "VALUES " +
                            "(@reportId," +
                            "@categoryId, @categoryLevel)";
            foreach ((int, int) categoryId in categoryIds)
            {
                List<CommandParameter> oParams = new List<CommandParameter>();
                oParams.Add(new CommandParameter("@reportId", CommandParameter.ParameterType.tInt, reportId));
                oParams.Add(new CommandParameter("@categoryId", CommandParameter.ParameterType.tInt, categoryId.Item1));
                oParams.Add(new CommandParameter("@categoryLevel", CommandParameter.ParameterType.tInt, categoryId.Item2));
                AccessHelper.ExecuteSql(sql, oParams);
            }

            return true;
        }

        public bool DeleteCategories(int reportId)
        {
            string sql = "@DELETE# " +
                         "FROM ReportLayoutCategories " +
                         "WHERE ReportLayoutId = @reportId";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@reportId", CommandParameter.ParameterType.tInt, reportId));

            return AccessHelper.ExecuteSql(sql, oParams);
        }

        #endregion Methods
    }
}