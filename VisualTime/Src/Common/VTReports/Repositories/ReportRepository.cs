using DevExpress.Xpo;
using Robotics.Base;
using Robotics.Base.DTOs;
using Robotics.DataLayer;
using Robotics.VTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ReportGenerator.Repositories
{
    public class ReportRepository : IReportRepository
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ReportRepository"/> to handle Database connections given certain <see cref="IDbConnection"/>.
        /// </summary>
        /// <param name="conectionInfo"> The connection information to link the <see cref="SqlConnection"/> with the desired Database instance.</param>
        public ReportRepository()
        {
        }

        #endregion Constructors

        #region Methods

        #region Devexpress

        public bool SetReportLayoutData(Report oReport)
        {
            var result = false;

            //System Reports
            if (oReport.CreatorPassportId == 0)
            {
                oReport.CreatorPassportId = null;
            }

            if (oReport.Id <= 0)
            {
                var identity = InsertAndGetId(oReport);
                if (identity > 0) result = true;
            }
            else
            {
                result = Update(oReport);
            }

            return result;
        }

        #endregion Devexpress

        public Report Get(int reportId)
        {
            Report report = null;
            string sql = "@SELECT# * FROM ReportLayouts where ID= @reportId And ISNULL(Visible,1) = 1";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@reportId", CommandParameter.ParameterType.tInt, reportId));
            DataTable resTable = AccessHelper.CreateDataTable(sql, oParams);

            if (resTable != null && resTable.Rows.Count > 0)
            {
                report = new Report()
                {
                    Id = reportId,
                    CreatorPassportId = roTypes.Any2Integer(resTable.Rows[0]["IdPassport"]),
                    Name = roTypes.Any2String(resTable.Rows[0]["LayoutName"]),
                    Description = roTypes.Any2String(resTable.Rows[0]["Description"]),
                    CreationDate = roTypes.Any2DateTime(resTable.Rows[0]["CreationDate"]),
                    IsEmergencyReport = roTypes.Any2Boolean(resTable.Rows[0]["IsEmergencyReport"]),
                    LayoutXMLBinary = (byte[])resTable.Rows[0]["LayoutXMLBinary"] ?? null,
                    LayoutPreviewXMLBinary = (byte[])resTable.Rows[0]["LayoutPreviewXMLBinary"] ?? null,
                    RequieredFeature = roTypes.Any2String(resTable.Rows[0]["RequieredFeature"]),
                    RequiredFunctionalities = roTypes.Any2String(resTable.Rows[0]["RequiredFunctionalities"]),
                    ParametersJson = roTypes.Any2String(resTable.Rows[0]["Parameters"])
                };
                if (report.CreatorPassportId == 0) report.CreatorPassportId = null;
            };
            return report;
        }

        public Report Get(string reportName)
        {
            Report report = null;
            string sql = "@SELECT# * FROM ReportLayouts where LayoutName= @reportName And ISNULL(Visible,1) = 1";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@reportName", CommandParameter.ParameterType.tString, reportName));
            DataTable resTable = AccessHelper.CreateDataTable(sql, oParams);

            if (resTable != null && resTable.Rows.Count > 0)
            {
                report = new Report()
                {
                    Id = roTypes.Any2Integer(resTable.Rows[0]["Id"]),
                    CreatorPassportId = roTypes.Any2Integer(resTable.Rows[0]["IdPassport"]),
                    Name = roTypes.Any2String(resTable.Rows[0]["LayoutName"]),
                    Description = roTypes.Any2String(resTable.Rows[0]["Description"]),
                    CreationDate = roTypes.Any2DateTime(resTable.Rows[0]["CreationDate"]),
                    IsEmergencyReport = roTypes.Any2Boolean(resTable.Rows[0]["IsEmergencyReport"]),
                    LayoutXMLBinary = (byte[])resTable.Rows[0]["LayoutXMLBinary"] ?? null,
                    LayoutPreviewXMLBinary = (byte[])resTable.Rows[0]["LayoutPreviewXMLBinary"] ?? null,
                    RequieredFeature = roTypes.Any2String(resTable.Rows[0]["RequieredFeature"]),
                    RequiredFunctionalities = roTypes.Any2String(resTable.Rows[0]["RequiredFunctionalities"]),
                    ParametersJson = roTypes.Any2String(resTable.Rows[0]["Parameters"])
                };
                if (report.CreatorPassportId == 0) report.CreatorPassportId = null;
            };
            return report;
        }

        public Report GetEmergencyReport()
        {
            Report report = null;
            string sql = "@SELECT# * FROM ReportLayouts where IsEmergencyReport = 1 And ISNULL(Visible,1) = 1";
            DataTable resTable = AccessHelper.CreateDataTable(sql);

            if (resTable != null && resTable.Rows.Count > 0)
            {
                report = new Report()
                {
                    Id = roTypes.Any2Integer(resTable.Rows[0]["Id"]),
                    CreatorPassportId = roTypes.Any2Integer(resTable.Rows[0]["IdPassport"]),
                    Name = roTypes.Any2String(resTable.Rows[0]["LayoutName"]),
                    Description = roTypes.Any2String(resTable.Rows[0]["Description"]),
                    CreationDate = roTypes.Any2DateTime(resTable.Rows[0]["CreationDate"]),
                    IsEmergencyReport = roTypes.Any2Boolean(resTable.Rows[0]["IsEmergencyReport"]),
                    LayoutXMLBinary = (byte[])resTable.Rows[0]["LayoutXMLBinary"] ?? null,
                    LayoutPreviewXMLBinary = (byte[])resTable.Rows[0]["LayoutPreviewXMLBinary"] ?? null,
                    RequieredFeature = roTypes.Any2String(resTable.Rows[0]["RequieredFeature"]),
                    RequiredFunctionalities = roTypes.Any2String(resTable.Rows[0]["RequiredFunctionalities"]),
                    ParametersJson = roTypes.Any2String(resTable.Rows[0]["Parameters"])
                };
                if (report.CreatorPassportId == 0) report.CreatorPassportId = null;
            };
            return report;
        }

        public List<Report> GetByPassportCategories(int passportId)
        {
            string sql = "@SELECT# r.ID AS Id, r.LayoutName AS Name, r.Description, r.LayoutXMLBinary,  r.LayoutPreviewXMLBinary " +
                                " ,r.CreationDate, r.IsEmergencyReport, r.IdPassport AS CreatorPassportId, r.Parameters AS ParametersJson, r.RequieredFeature AS RequieredFeature, r.RequiredFunctionalities as RequiredFunctionalities " +
                        " FROM ReportLayouts AS r " +
                        " LEFT JOIN ReportLayoutCategories ON r.Id = ReportLayoutCategories.ReportLayoutId " +
                        " WHERE ((ReportLayoutCategories.CategoryId IN(@SELECT# IDCategory FROM sysroPassports_Categories WHERE IDPassport = @passportId)) " +
                        " OR (NOT EXISTS (@SELECT# rlc.ReportLayoutId FROM ReportLayoutCategories AS rlc WHERE rlc.ReportLayoutId = r.ID))) AND ISNULL(r.Visible,1) = 1";

            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@passportId", CommandParameter.ParameterType.tInt, passportId));
            DataTable resTable = AccessHelper.CreateDataTable(sql, oParams);

            List<Report> reports = new List<Report>();

            if (resTable != null && resTable.Rows.Count > 0)
            {
                foreach (DataRow row in resTable.Rows)
                {
                    Report report = new Report()
                    {
                        Id = roTypes.Any2Integer(row["Id"]),
                        CreatorPassportId = roTypes.Any2Integer(row["CreatorPassportId"]),
                        Name = roTypes.Any2String(row["Name"]),
                        Description = roTypes.Any2String(row["Description"]),
                        CreationDate = roTypes.Any2DateTime(row["CreationDate"]),
                        IsEmergencyReport = roTypes.Any2Boolean(row["IsEmergencyReport"]),
                        LayoutXMLBinary = (byte[])resTable.Rows[0]["LayoutXMLBinary"] ?? null,
                        LayoutPreviewXMLBinary = (byte[])resTable.Rows[0]["LayoutPreviewXMLBinary"] ?? null,
                        RequieredFeature = roTypes.Any2String(row["RequieredFeature"]),
                        RequiredFunctionalities = roTypes.Any2String(row["RequiredFunctionalities"]),
                        ParametersJson = roTypes.Any2String(row["ParametersJson"])
                    };
                    if (report.CreatorPassportId == 0) report.CreatorPassportId = null;
                    reports.Add(report);
                }
            }

            string sqlPlanneds = " @SELECT# Id, ReportId " +
                                 " FROM ReportPlannedExecutions " +
                                 " WHERE ReportId in (@SELECT# id FROM ReportLayouts AS r WHERE IDPassport = @passportId OR IDPassport is null) " +
                                 "       AND  ( PassportID =  @passportId  OR PassportID IS NULL )";

            List<CommandParameter> oParamsPlanned = new List<CommandParameter>();
            oParamsPlanned.Add(new CommandParameter("@passportId", CommandParameter.ParameterType.tInt, passportId));
            DataTable resTablePlanned = AccessHelper.CreateDataTable(sqlPlanneds, oParamsPlanned);

            List<ReportPlannedExecution> plannedList = new List<ReportPlannedExecution>();

            if (resTablePlanned != null && resTablePlanned.Rows.Count > 0)
            {
                foreach (DataRow row in resTablePlanned.Rows)
                {
                    ReportPlannedExecution planned = new ReportPlannedExecution()
                    {
                        ReportId = roTypes.Any2Integer(row["ReportId"])
                    };
                    plannedList.Add(planned);
                }
            }

            foreach (Report report in reports)
            {
                report.PlannedExecutionsList = new List<ReportPlannedExecution>();
                report.HasExecutionsPlanned = false;
                report.CategoriesDescription = string.Empty;
                foreach (ReportPlannedExecution planned in plannedList)
                {
                    if (report.Id.Equals(planned.ReportId))
                    {
                        ReportPlannedExecution plannedReport = new ReportPlannedExecution();
                        plannedReport.ReportId = planned.ReportId;
                        report.PlannedExecutionsList.Add(plannedReport);
                        report.HasExecutionsPlanned = true;
                    }
                }
            }

            return reports;
        }

        public bool Insert(Report report)
        {
            if (!Robotics.DataLayer.roSupport.IsXSSSafe(report))
            {
                return false;
            }

            string query = "@INSERT# INTO [dbo].[ReportLayouts] ([LayoutName],[Description],[LayoutXMLBinary],[IdPassport],[CreationDate],[LayoutPreviewXMLBinary],[Parameters],[IsEmergencyReport],[RequieredFeature],[Visible]) " +
                           "VALUES                            (@layoutname,@description,@layoutxmlbinary,@idpassport,@creationdate,@layoutpreviewxmlbinary,@parameters,@isemergencyreport,@requieredfeature,1)";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@layoutname", CommandParameter.ParameterType.tString, report.Name));
            oParams.Add(new CommandParameter("@description", CommandParameter.ParameterType.tString, report.Description));
            oParams.Add(new CommandParameter("@layoutxmlbinary", CommandParameter.ParameterType.tVarBinary, report.LayoutXMLBinary ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@idpassport", CommandParameter.ParameterType.tInt, report.CreatorPassportId ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@creationdate", CommandParameter.ParameterType.tDateTime, report.CreationDate));
            oParams.Add(new CommandParameter("@layoutpreviewxmlbinary", CommandParameter.ParameterType.tVarBinary, report.LayoutPreviewXMLBinary ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@parameters", CommandParameter.ParameterType.tString, report.ParametersJson));
            oParams.Add(new CommandParameter("@isemergencyreport", CommandParameter.ParameterType.tBoolean, report.IsEmergencyReport));
            oParams.Add(new CommandParameter("@requieredfeature", CommandParameter.ParameterType.tString, report.RequieredFeature == null ? string.Empty : report.RequieredFeature));
            oParams.Add(new CommandParameter("@requiredfunctionalities", CommandParameter.ParameterType.tString, report.RequiredFunctionalities == null ? string.Empty : report.RequiredFunctionalities));

            return AccessHelper.ExecuteSql(query, oParams);
        }

        public int InsertAndGetId(Report report)
        {
            if (!Robotics.DataLayer.roSupport.IsXSSSafe(report))
            {
                return 0;
            }

            string query = "@INSERT# INTO [dbo].[ReportLayouts] ([LayoutName],[Description],[LayoutXMLBinary],[IdPassport],[CreationDate],[LayoutPreviewXMLBinary],[Parameters],[IsEmergencyReport],[RequieredFeature],[Visible]) " +
                           "OUTPUT INSERTED.Id VALUES                            (@layoutname,@description,@layoutxmlbinary,@idpassport,@creationdate,@layoutpreviewxmlbinary,@parameters,@isemergencyreport,@requieredfeature,1)";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@layoutname", CommandParameter.ParameterType.tString, report.Name));
            oParams.Add(new CommandParameter("@description", CommandParameter.ParameterType.tString, report.Description));
            oParams.Add(new CommandParameter("@layoutxmlbinary", CommandParameter.ParameterType.tVarBinary, report.LayoutXMLBinary ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@idpassport", CommandParameter.ParameterType.tInt, report.CreatorPassportId));
            oParams.Add(new CommandParameter("@creationdate", CommandParameter.ParameterType.tDateTime, report.CreationDate));
            oParams.Add(new CommandParameter("@layoutpreviewxmlbinary", CommandParameter.ParameterType.tVarBinary, report.LayoutPreviewXMLBinary ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@parameters", CommandParameter.ParameterType.tString, report.ParametersJson));
            oParams.Add(new CommandParameter("@isemergencyreport", CommandParameter.ParameterType.tBoolean, report.IsEmergencyReport));
            oParams.Add(new CommandParameter("@requieredfeature", CommandParameter.ParameterType.tString, report.RequieredFeature == null ? string.Empty : report.RequieredFeature));
            oParams.Add(new CommandParameter("@requiredfunctionalities", CommandParameter.ParameterType.tString, report.RequiredFunctionalities == null ? string.Empty : report.RequiredFunctionalities));

            int insertedId = (int)AccessHelper.ExecuteScalar(query, oParams);
            return insertedId;
        }

        public bool Update(Report report)
        {
            if (!Robotics.DataLayer.roSupport.IsXSSSafe(report))
            {
                return false;
            }
            string query = "@UPDATE# [dbo].[ReportLayouts] " +
                           "SET [LayoutName] = @layoutname " +
                           ",[Description] = @description ";
            if (report.LayoutXMLBinary != null) query += ",[LayoutXMLBinary] = @layoutxmlbinary ";
            query += ",[IdPassport] = @idpassport " +
                    ",[CreationDate] = @creationdate ";
            if (report.LayoutPreviewXMLBinary != null) query += ",[LayoutPreviewXMLBinary] = @layoutpreviewxmlbinary ";
            query += ",[Parameters] = @parameters " +
                    ",[IsEmergencyReport] = @isemergencyreport " +
                    ",[RequieredFeature] = @requieredfeature " +
                    "WHERE Id = @id AND ISNULL(Visible,1) = 1";

            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@id", CommandParameter.ParameterType.tInt, report.Id));
            oParams.Add(new CommandParameter("@layoutname", CommandParameter.ParameterType.tString, report.Name));
            oParams.Add(new CommandParameter("@description", CommandParameter.ParameterType.tString, report.Description));
            if (report.LayoutXMLBinary != null) oParams.Add(new CommandParameter("@layoutxmlbinary", CommandParameter.ParameterType.tVarBinary, report.LayoutXMLBinary ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@idpassport", CommandParameter.ParameterType.tInt, report.CreatorPassportId ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@creationdate", CommandParameter.ParameterType.tDateTime, report.CreationDate));
            if (report.LayoutPreviewXMLBinary != null) oParams.Add(new CommandParameter("@layoutpreviewxmlbinary", CommandParameter.ParameterType.tVarBinary, report.LayoutPreviewXMLBinary ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@parameters", CommandParameter.ParameterType.tString, report.ParametersJson));
            oParams.Add(new CommandParameter("@isemergencyreport", CommandParameter.ParameterType.tBoolean, report.IsEmergencyReport));
            oParams.Add(new CommandParameter("@requieredfeature", CommandParameter.ParameterType.tString, report.RequieredFeature));
            oParams.Add(new CommandParameter("@requiredfunctionalities", CommandParameter.ParameterType.tString, report.RequiredFunctionalities));
            return AccessHelper.ExecuteSql(query, oParams);
        }

        public bool Delete(int reportId)
        {
            string query = "@DELETE# FROM [dbo].[ReportLayouts] WHERE Id = @id And ISNULL(Visible,1) = 1";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@id", CommandParameter.ParameterType.tInt, reportId));

            return AccessHelper.ExecuteSql(query, oParams);
        }

        #endregion Methods
    }
}