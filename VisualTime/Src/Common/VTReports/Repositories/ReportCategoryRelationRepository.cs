using DapperExtensions;
using ReportGenerator.Domain;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using Robotics.Base;
using DapperExtensions.Mapper;
using System.Dynamic;

namespace ReportGenerator.Repositories
{
    public class ReportCategoryRelationRepository : IReportCategoryRelationRepository
    {
        private DatabaseConnection databaseConnection;

        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="ReportCategoryRelationRepository"/> to handle Database connections given certain <see cref="IDbConnection"/>.
        /// </summary>
        /// <param name="conectionInfo"> The connection information to link the <see cref="SqlConnection"/> with the desired Database instance.</param>
        public ReportCategoryRelationRepository(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }
        #endregion

        #region Methods
        public List<int> GetCategoryIds(int reportId)
        {
            string sql = "SELECT " +
                             "CategoryId " +
                         "FROM ReportLayoutCategories " +
                         "WHERE ReportLayoutId = @reportId ";

            return databaseConnection.connection.Query<int>(sql, new { reportId = reportId }).AsList();

        }

        public bool Insert(int reportId, IEnumerable<int> categoryIds)
        {
            string sql = "INSERT " +
                         "INTO ReportLayoutCategories " +
                         "VALUES " +
                            "(@reportId," +
                            "@categoryId)";

            foreach (int auxiliarCategoryId in categoryIds)
            {
                databaseConnection.connection.Execute(sql, new { reportId = reportId, categoryId = auxiliarCategoryId }, this.databaseConnection.transaction);
            }

            return true;
        }

        public bool Delete(int reportId, IEnumerable<int> categoryIds)
        {
            string sql = "DELETE " +
                         "FROM ReportLayoutCategories " +
                         "WHERE ReportLayoutId = @reportId " +
                            "AND CategoryId IN @categoryIds";

            databaseConnection.connection.Execute(sql, new { reportId = reportId, categoryIds = categoryIds }, this.databaseConnection.transaction);

            return true;
        }

        public bool DeleteAll(int reportId)
        {
            string sql = "DELETE " +
                         "FROM ReportLayoutCategories " +
                         "WHERE ReportLayoutId = @reportId";

            databaseConnection.connection.Execute(sql, new { reportId = reportId }, this.databaseConnection.transaction);

            return true;
        }
        #endregion
    }
}