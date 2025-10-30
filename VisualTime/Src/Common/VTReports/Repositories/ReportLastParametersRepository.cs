using Robotics.DataLayer;
using Robotics.VTBase;
using System.Collections.Generic;
using System.Data;

namespace ReportGenerator.Repositories
{
    public class ReportLastParametersRepository : IReportLastParametersRepository
    {
        #region Constructors

        public ReportLastParametersRepository()
        {
        }

        #endregion Constructors

        #region Methods

        public string Get(int reportId, int passportId)
        {
            string result = null;
            string sql = "@SELECT# Parameters FROM ReportExecutionsLastParameters WHERE ReportId = @reportId AND PassportId = @passportId";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@reportId", CommandParameter.ParameterType.tInt, reportId));
            oParams.Add(new CommandParameter("@passportId", CommandParameter.ParameterType.tInt, passportId));

            DataTable resTable = AccessHelper.CreateDataTable(sql, oParams);
            if (resTable != null && resTable.Rows.Count > 0)
            {
                result = roTypes.Any2String(resTable.Rows[0]["Parameters"]);
            }
            return result;
        }

        public bool Insert(int reportId, int passportId, string lastParameters)
        {
            string sql = "@INSERT# INTO ReportExecutionsLastParameters " +
                         "VALUES (@passportId, @reportId, @lastParameters)";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@reportId", CommandParameter.ParameterType.tInt, reportId));
            oParams.Add(new CommandParameter("@passportId", CommandParameter.ParameterType.tInt, passportId));
            oParams.Add(new CommandParameter("@lastParameters", CommandParameter.ParameterType.tString, lastParameters));

            return AccessHelper.ExecuteSql(sql, oParams);
        }

        public bool Update(int reportId, int passportId, string lastParameters)
        {
            string sql = "@UPDATE# ReportExecutionsLastParameters " +
                         "SET Parameters = @lastParameters " +
                         "WHERE PassportId = @passportId AND ReportId = @reportId";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@reportId", CommandParameter.ParameterType.tInt, reportId));
            oParams.Add(new CommandParameter("@passportId", CommandParameter.ParameterType.tInt, passportId));
            oParams.Add(new CommandParameter("@lastParameters", CommandParameter.ParameterType.tString, lastParameters));

            return AccessHelper.ExecuteSql(sql, oParams);
        }

        public bool Delete(int reportId, int passportId)
        {
            string sql = "@DELETE# FROM ReportExecutionsLastParameters " +
                         "WHERE PassportId = @passportId AND ReportId = @reportId";

            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@reportId", CommandParameter.ParameterType.tInt, reportId));
            oParams.Add(new CommandParameter("@passportId", CommandParameter.ParameterType.tInt, passportId));

            return AccessHelper.ExecuteSql(sql, oParams);
        }

        public bool DeleteAll(int reportId)
        {
            string sql = "@DELETE# FROM ReportExecutionsLastParameters " +
                         "WHERE ReportId = @reportId";

            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@reportId", CommandParameter.ParameterType.tInt, reportId));

            return AccessHelper.ExecuteSql(sql, oParams);
        }

        #endregion Methods
    }
}