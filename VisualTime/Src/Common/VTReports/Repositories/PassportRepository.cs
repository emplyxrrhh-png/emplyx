using Robotics.Base;
using Robotics.DataLayer;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ReportGenerator.Repositories
{
    public class PassportRepository : IPassportRepository
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="PassportRepository"/> to handle Database connections given certain <see cref="IDbConnection"/>.
        /// </summary>
        /// <param name="conectionInfo"> The connection information to link the <see cref="SqlConnection"/> with the desired Database instance.</param>
        public PassportRepository()
        {
        }

        #endregion Constructors

        #region Methods

        public ReportPassport Get(int passportId)
        {
            ReportPassport passport = null;
            string sql = "@SELECT# Name, Description FROM sysroPassports where id= @passportID";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@passportID", CommandParameter.ParameterType.tInt, passportId));
            DataTable resTable = AccessHelper.CreateDataTable(sql, oParams);

            if (resTable != null && resTable.Rows.Count > 0)
            {
                passport = new ReportPassport();
                passport.Name = (string)resTable.Rows[0]["Name"];
                passport.Description = (string)resTable.Rows[0]["Description"];
            }
            return passport;
        }

        #endregion Methods
    }
}