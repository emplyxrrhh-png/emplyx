using Robotics.Base;
using Robotics.DataLayer;
using Robotics.VTBase;
using System;
using System.Collections.Generic;
using System.Data;

namespace ReportGenerator.Repositories
{
    public class ReportExecutionRepository : IReportExecutionRepository
    {
        #region Constructors

        public ReportExecutionRepository()
        {
        }

        #endregion Constructors

        #region Methods

        public ReportExecution Get(Guid guid)
        {
            ReportExecution reportExecution = null;
            string sql = "@SELECT# * FROM ReportExecutions where Guid= @guid";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@guid", CommandParameter.ParameterType.tString, guid.ToString()));
            DataTable resTable = AccessHelper.CreateDataTable(sql, oParams);

            if (resTable != null && resTable.Rows.Count > 0)
            {
                DataRow row = resTable.Rows[0];
                reportExecution = new ReportExecution()
                {
                    Guid = guid,
                    Status = roTypes.Any2Integer(row["Status"]),
                    ReportID = roTypes.Any2Integer(row["LayoutID"]),
                    PassportID = roTypes.Any2Integer(row["PassportID"]),
                    ExecutionDate = row["ExecutionDate"] is DBNull ? null : (DateTime?)row["ExecutionDate"],
                    FileLink = roTypes.Any2String(row["FileLink"]),
                    Format = (ReportExportExtensions)Enum.Parse(typeof(ReportExportExtensions), row["Format"] is DBNull ? "0" : roTypes.Any2String(row["Format"])),
                };
            }
            return reportExecution;
        }

        public List<ReportExecution> Get(int layoutID)
        {
            List<ReportExecution> reportExecution = new List<ReportExecution>();
            string sql = "@SELECT# * FROM ReportExecutions where LayoutID= @layoutid";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@layoutid", CommandParameter.ParameterType.tInt, layoutID));
            DataTable resTable = AccessHelper.CreateDataTable(sql, oParams);

            if (resTable != null && resTable.Rows.Count > 0)
            {
                foreach (DataRow row in resTable.Rows)
                {
                    reportExecution.Add(new ReportExecution()
                    {
                        Guid = Guid.Parse(roTypes.Any2String(row["Guid"])),
                        Status = roTypes.Any2Integer(row["Status"]),
                        ReportID = roTypes.Any2Integer(row["LayoutID"]),
                        PassportID = roTypes.Any2Integer(row["PassportID"]),
                        ExecutionDate = row["ExecutionDate"] is DBNull ? null : (DateTime?)row["ExecutionDate"],
                        FileLink = roTypes.Any2String(row["FileLink"]),
                        Format = (ReportExportExtensions)Enum.Parse(typeof(ReportExportExtensions), row["Format"] is DBNull ? "0" : roTypes.Any2String(row["Format"])),
                    });
                }
            }

            return reportExecution;
        }

        public List<ReportExecution> Get(int layoutID, int passportId)
        {
            List<ReportExecution> reportExecutions = new List<ReportExecution>();
            string sql = "@SELECT# * FROM ReportExecutions where LayoutID= @layoutid AND PassportID = @passportid";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@layoutid", CommandParameter.ParameterType.tInt, layoutID));
            oParams.Add(new CommandParameter("@passportid", CommandParameter.ParameterType.tInt, passportId));
            DataTable resTable = AccessHelper.CreateDataTable(sql, oParams);

            if (resTable != null && resTable.Rows.Count > 0)
            {
                foreach (DataRow row in resTable.Rows)
                {
                    reportExecutions.Add(new ReportExecution()
                    {
                        Guid = Guid.Parse(roTypes.Any2String(row["Guid"])),
                        Status = roTypes.Any2Integer(row["Status"]),
                        ReportID = roTypes.Any2Integer(row["LayoutID"]),
                        PassportID = roTypes.Any2Integer(row["PassportID"]),
                        ExecutionDate = row["ExecutionDate"] is DBNull ? null : (DateTime?)row["ExecutionDate"],
                        FileLink = roTypes.Any2String(row["FileLink"]),
                        Format = (ReportExportExtensions)Enum.Parse(typeof(ReportExportExtensions), row["Format"] is DBNull ? "0" : roTypes.Any2String(row["Format"])),
                    });
                }
            }

            return reportExecutions;
        }

        public List<ReportExecution> Get(IEnumerable<Guid> executionIds)
        {
            List<ReportExecution> reportExecutions = new List<ReportExecution>();
            foreach (var executionId in executionIds)
            {
                var reportExecution = Get(executionId);
                if (reportExecution != null)
                {
                    reportExecutions.Add(reportExecution);
                }
            }
            return reportExecutions;
        }

        public Guid Insert(ReportExecution reportExecution)
        {
            string query = "@INSERT# INTO ReportExecutions (LayoutID, FileLink, PassportID, Guid, Status, ExecutionDate, Format) OUTPUT INSERTED.Guid VALUES (@LayoutID, @FileLink, @PassportID, @Guid, @Status, @ExecutionDate, @Format)";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@LayoutID", CommandParameter.ParameterType.tInt, reportExecution.ReportID));
            oParams.Add(new CommandParameter("@FileLink", CommandParameter.ParameterType.tString, reportExecution.FileLink == null ? string.Empty : reportExecution.FileLink));
            oParams.Add(new CommandParameter("@PassportID", CommandParameter.ParameterType.tInt, reportExecution.PassportID));
            oParams.Add(new CommandParameter("@Guid", CommandParameter.ParameterType.tString, reportExecution.Guid.ToString()));
            oParams.Add(new CommandParameter("@Status", CommandParameter.ParameterType.tInt, reportExecution.Status));
            oParams.Add(new CommandParameter("@ExecutionDate", CommandParameter.ParameterType.tDateTime, reportExecution.ExecutionDate ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@Format", CommandParameter.ParameterType.tInt, (int)reportExecution.Format));

            Guid insertedGuid = (Guid)AccessHelper.ExecuteScalar(query, oParams);
            return insertedGuid;
        }

        public bool Update(ReportExecution reportExecution)
        {
            string query = "@UPDATE# ReportExecutions SET LayoutID = @LayoutID, FileLink = @FileLink, PassportID = @PassportID, Status = @Status, ExecutionDate = @ExecutionDate, Format = @Format WHERE Guid = @Guid";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@LayoutID", CommandParameter.ParameterType.tInt, reportExecution.ReportID));
            oParams.Add(new CommandParameter("@FileLink", CommandParameter.ParameterType.tString, reportExecution.FileLink == null ? string.Empty : reportExecution.FileLink));
            oParams.Add(new CommandParameter("@PassportID", CommandParameter.ParameterType.tInt, reportExecution.PassportID));
            oParams.Add(new CommandParameter("@Status", CommandParameter.ParameterType.tInt, reportExecution.Status));
            oParams.Add(new CommandParameter("@ExecutionDate", CommandParameter.ParameterType.tDateTime, reportExecution.ExecutionDate ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@Format", CommandParameter.ParameterType.tInt, (int)reportExecution.Format));
            oParams.Add(new CommandParameter("@Guid", CommandParameter.ParameterType.tString, reportExecution.Guid.ToString()));

            return AccessHelper.ExecuteSql(query, oParams);
        }

        public bool Delete(Guid executionId)
        {
            string query = "@DELETE# ReportExecutions WHERE Guid = @Guid";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@Guid", CommandParameter.ParameterType.tString, executionId.ToString()));

            return AccessHelper.ExecuteSql(query, oParams);
        }

        public bool Delete(IEnumerable<Guid> executionIds)
        {
            //TODO_DAPPER: Transaction
            Boolean bolret = false;
            foreach (var executionId in executionIds)
            {
                bolret &= Delete(executionId);
            }
            return bolret;
        }

        public bool DeleteAll(int layoudId)
        {
            string query = "@DELETE# ReportExecutions WHERE LayoutID = @layoutid";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@layoutid", CommandParameter.ParameterType.tInt, layoudId));

            return AccessHelper.ExecuteSql(query, oParams);
        }

        #endregion Methods
    }
}