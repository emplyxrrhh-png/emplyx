using DevExpress.XtraReports;
using Robotics.Base;
using Robotics.DataLayer;
using Robotics.VTBase;
using System;
using System.Collections.Generic;
using System.Data;

namespace ReportGenerator.Repositories
{
    public class ReportPlannedExecutionRepository : IReportPlannedExecutionRepository
    {
        #region Constructors

        public ReportPlannedExecutionRepository()
        {
        }

        #endregion Constructors

        #region Methods

        public ReportPlannedExecution GetPlannedExecution(int plannedExecutionId)
        {
            ReportPlannedExecution reportPlannedExecution = null;
            string sql = "@SELECT# * FROM ReportPlannedExecutions where Id= @plannedExecutionId";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@plannedExecutionId", CommandParameter.ParameterType.tInt, plannedExecutionId));
            DataTable resTable = AccessHelper.CreateDataTable(sql, oParams);

            if (resTable != null && resTable.Rows.Count > 0)
            {
                DataRow row = resTable.Rows[0];
                reportPlannedExecution = new ReportPlannedExecution()
                {
                    Id = plannedExecutionId,
                    ReportId = roTypes.Any2Integer(row["ReportId"]),
                    PassportId = roTypes.Any2Integer(row["PassportId"]),
                    Language = roTypes.Any2String(row["Language"]),
                    Scheduler = roTypes.Any2String(row["Scheduler"]),
                    Destination = roTypes.Any2String(row["Destination"]),
                    Format = (ReportExportExtensions)Enum.Parse(typeof(ReportExportExtensions), row["Format"] is DBNull ? "0" : roTypes.Any2String(row["Format"])),
                    ParametersJson = roTypes.Any2String(row["Parameters"]),
                    NextExecutionDate = row["NextExecutionDate"] is DBNull ? null : (DateTime?)row["NextExecutionDate"],
                    LastExecutionDate = roTypes.Any2DateTime(row["LastExecutionDate"]),
                    RegisteredPlannedExecutionDate = roTypes.Any2DateTime(row["RegisteredPlannedExecutionDate"]),
                    ViewFields = roTypes.Any2String(row["ViewFields"])
                };
            }

            return reportPlannedExecution;
        }

        public List<ReportPlannedExecution> GetPlannedExecutions(int reportId)
        {
            List<ReportPlannedExecution> reportPlannedExecutions = new List<ReportPlannedExecution>();
            string sql = "@SELECT# * FROM ReportPlannedExecutions where ReportId= @reportid ORDER BY RegisteredPlannedExecutionDate ASC";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@reportid", CommandParameter.ParameterType.tInt, reportId));
            DataTable resTable = AccessHelper.CreateDataTable(sql, oParams);

            if (resTable != null && resTable.Rows.Count > 0)
            {
                reportPlannedExecutions = new List<ReportPlannedExecution>();
                foreach (DataRow row in resTable.Rows)
                {
                    reportPlannedExecutions.Add(new ReportPlannedExecution()
                    {
                        Id = roTypes.Any2Integer(row["Id"]),
                        ReportId = roTypes.Any2Integer(row["ReportId"]),
                        PassportId = roTypes.Any2Integer(row["PassportId"]),
                        Language = roTypes.Any2String(row["Language"]),
                        Scheduler = roTypes.Any2String(row["Scheduler"]),
                        Destination = string.IsNullOrEmpty(roTypes.Any2String(row["Destination"])) ? "{}" : roTypes.Any2String(row["Destination"]),
                        Format = (ReportExportExtensions)Enum.Parse(typeof(ReportExportExtensions), row["Format"] is DBNull ? "0" : roTypes.Any2String(row["Format"])),
                        ParametersJson = roTypes.Any2String(row["Parameters"]),
                        NextExecutionDate = row["NextExecutionDate"] is DBNull ? null : (DateTime?)row["NextExecutionDate"],
                        LastExecutionDate = roTypes.Any2DateTime(row["LastExecutionDate"]),
                        RegisteredPlannedExecutionDate = roTypes.Any2DateTime(row["RegisteredPlannedExecutionDate"]),
                        ViewFields = roTypes.Any2String(row["ViewFields"])
                    });
                }
            }
            return reportPlannedExecutions;
        }

        public List<ReportPlannedExecution> GetPlannedExecutions(int reportId, int passportId)
        {
            List<ReportPlannedExecution> reportPlannedExecutions = null;
            string sql = "@SELECT# * FROM ReportPlannedExecutions where ReportId= @reportid AND PassportId = @passportid ORDER BY RegisteredPlannedExecutionDate ASC";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@reportid", CommandParameter.ParameterType.tInt, reportId));
            oParams.Add(new CommandParameter("@passportid", CommandParameter.ParameterType.tInt, passportId));
            DataTable resTable = AccessHelper.CreateDataTable(sql, oParams);
            reportPlannedExecutions = new List<ReportPlannedExecution>();
            if (resTable != null && resTable.Rows.Count > 0)
            {
                foreach (DataRow row in resTable.Rows)
                {
                    reportPlannedExecutions.Add(new ReportPlannedExecution()
                    {
                        Id = roTypes.Any2Integer(row["Id"]),
                        ReportId = roTypes.Any2Integer(row["ReportId"]),
                        PassportId = roTypes.Any2Integer(row["PassportId"]),
                        Language = roTypes.Any2String(row["Language"]),
                        Scheduler = roTypes.Any2String(row["Scheduler"]),
                        Destination = roTypes.Any2String(row["Destination"]),
                        Format = (ReportExportExtensions)Enum.Parse(typeof(ReportExportExtensions), row["Format"] is DBNull ? "0" : roTypes.Any2String(row["Format"])),
                        ParametersJson = roTypes.Any2String(row["Parameters"]),
                        NextExecutionDate = row["NextExecutionDate"] is DBNull ? null : (DateTime?)row["NextExecutionDate"],
                        LastExecutionDate = roTypes.Any2DateTime(row["LastExecutionDate"]),
                        RegisteredPlannedExecutionDate = roTypes.Any2DateTime(row["RegisteredPlannedExecutionDate"]),
                        ViewFields = roTypes.Any2String(row["ViewFields"])
                    });
                }
            }
            return reportPlannedExecutions;
        }

        public List<ReportPlannedExecution> GetToExecute()
        {
            List<ReportPlannedExecution> reportPlannedExecutions = null;
            string sql = "@SELECT# * FROM ReportPlannedExecutions where NextExecutionDate <= @now ORDER BY RegisteredPlannedExecutionDate ASC";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@now", CommandParameter.ParameterType.tDateTime, DateTime.Now));
            DataTable resTable = AccessHelper.CreateDataTable(sql, oParams);

            if (resTable != null && resTable.Rows.Count > 0)
            {
                reportPlannedExecutions = new List<ReportPlannedExecution>();
                foreach (DataRow row in resTable.Rows)
                {
                    reportPlannedExecutions.Add(new ReportPlannedExecution()
                    {
                        Id = roTypes.Any2Integer(row["Id"]),
                        ReportId = roTypes.Any2Integer(row["ReportId"]),
                        PassportId = roTypes.Any2Integer(row["PassportId"]),
                        Language = roTypes.Any2String(row["Language"]),
                        Scheduler = roTypes.Any2String(row["Scheduler"]),
                        Destination = roTypes.Any2String(row["Destination"]),
                        Format = (ReportExportExtensions)Enum.Parse(typeof(ReportExportExtensions), row["Format"] is DBNull ? "0" : roTypes.Any2String(row["Format"])),
                        ParametersJson = roTypes.Any2String(row["Parameters"]),
                        NextExecutionDate = row["NextExecutionDate"] is DBNull ? null : (DateTime?)row["NextExecutionDate"],
                        LastExecutionDate = roTypes.Any2DateTime(row["LastExecutionDate"]),
                        RegisteredPlannedExecutionDate = roTypes.Any2DateTime(row["RegisteredPlannedExecutionDate"]),
                        ViewFields = roTypes.Any2String(row["ViewFields"])
                    });
                }
            }
            return reportPlannedExecutions;
        }

        public bool Insert(ReportPlannedExecution reportPlannedExecution)
        {
            if (!Robotics.DataLayer.roSupport.IsXSSSafe(reportPlannedExecution))
            {
                return false;
            }
            string sql = @"@INSERT# INTO [dbo].[ReportPlannedExecutions]
                          ([ReportId],[PassportId],[Language],[Scheduler],[Destination],[Format],[Parameters],[NextExecutionDate],[LastExecutionDate],[RegisteredPlannedExecutionDate],[ViewFields])
                          VALUES
                          (@reportid,@passportid,@language,@scheduler,@destination,@format,@parameters,@nextExecutionDate,@lastExecutionDate,@registeredPlannedExecutionDate,@viewFields)";

            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@reportid", CommandParameter.ParameterType.tInt, reportPlannedExecution.ReportId));
            oParams.Add(new CommandParameter("@passportid", CommandParameter.ParameterType.tInt, reportPlannedExecution.PassportId));
            oParams.Add(new CommandParameter("@language", CommandParameter.ParameterType.tString, reportPlannedExecution.Language));
            oParams.Add(new CommandParameter("@scheduler", CommandParameter.ParameterType.tString, reportPlannedExecution.Scheduler));
            oParams.Add(new CommandParameter("@destination", CommandParameter.ParameterType.tString, reportPlannedExecution.Destination));
            oParams.Add(new CommandParameter("@format", CommandParameter.ParameterType.tInt, (int)reportPlannedExecution.Format));
            oParams.Add(new CommandParameter("@parameters", CommandParameter.ParameterType.tString, reportPlannedExecution.ParametersJson));
            oParams.Add(new CommandParameter("@nextExecutionDate", CommandParameter.ParameterType.tDateTime, reportPlannedExecution.NextExecutionDate ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@lastExecutionDate", CommandParameter.ParameterType.tDateTime, reportPlannedExecution.LastExecutionDate ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@registeredPlannedExecutionDate", CommandParameter.ParameterType.tDateTime, reportPlannedExecution.RegisteredPlannedExecutionDate));
            oParams.Add(new CommandParameter("@viewFields", CommandParameter.ParameterType.tString, reportPlannedExecution.ViewFields));

            return AccessHelper.ExecuteSql(sql, oParams);
        }

        public bool Insert(IEnumerable<ReportPlannedExecution> reportPlannedExecutions)
        {
            bool result = true;
            foreach (ReportPlannedExecution reportPlannedExecution in reportPlannedExecutions)
            {
                if (!Insert(reportPlannedExecution))
                {
                    result = false;
                }
            }
            return result;
        }

        public bool Update(ReportPlannedExecution reportPlannedExecution)
        {
            if (!Robotics.DataLayer.roSupport.IsXSSSafe(reportPlannedExecution))
            {
                return false;
            }
            string sql = @"@UPDATE# [dbo].[ReportPlannedExecutions]
                                    SET [ReportId] = @reportid
                                        ,[PassportId] = @passportid
                                        ,[Language] = @language
                                        ,[Scheduler] = @scheduler
                                        ,[Destination] = @destination
                                        ,[Format] = @format
                                        ,[Parameters] = @parameters
                                        ,[NextExecutionDate] = @nextExecutionDate
                                        ,[LastExecutionDate] = @lastExecutionDate
                                        ,[RegisteredPlannedExecutionDate] = @registeredPlannedExecutionDate
                                        ,[ViewFields] = @viewFields
                                    WHERE Id = @id";

            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@id", CommandParameter.ParameterType.tInt, reportPlannedExecution.Id));
            oParams.Add(new CommandParameter("@reportid", CommandParameter.ParameterType.tInt, reportPlannedExecution.ReportId));
            oParams.Add(new CommandParameter("@passportid", CommandParameter.ParameterType.tInt, reportPlannedExecution.PassportId));
            oParams.Add(new CommandParameter("@language", CommandParameter.ParameterType.tString, reportPlannedExecution.Language));
            oParams.Add(new CommandParameter("@scheduler", CommandParameter.ParameterType.tString, reportPlannedExecution.Scheduler));
            oParams.Add(new CommandParameter("@destination", CommandParameter.ParameterType.tString, reportPlannedExecution.Destination));
            oParams.Add(new CommandParameter("@format", CommandParameter.ParameterType.tInt, (int)reportPlannedExecution.Format));
            oParams.Add(new CommandParameter("@parameters", CommandParameter.ParameterType.tString, reportPlannedExecution.ParametersJson));
            oParams.Add(new CommandParameter("@nextExecutionDate", CommandParameter.ParameterType.tDateTime, reportPlannedExecution.NextExecutionDate ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@lastExecutionDate", CommandParameter.ParameterType.tDateTime, reportPlannedExecution.LastExecutionDate ?? (object)DBNull.Value));
            oParams.Add(new CommandParameter("@registeredPlannedExecutionDate", CommandParameter.ParameterType.tDateTime, reportPlannedExecution.RegisteredPlannedExecutionDate));
            oParams.Add(new CommandParameter("@viewFields", CommandParameter.ParameterType.tString, reportPlannedExecution.ViewFields));

            return AccessHelper.ExecuteSql(sql, oParams);
        }

        public bool Delete(int plannedExecutionId)
        {
            string sql = "@DELETE# ReportPlannedExecutions where Id= @plannedExecutionId";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@plannedExecutionId", CommandParameter.ParameterType.tInt, plannedExecutionId));

            return AccessHelper.ExecuteSql(sql, oParams);
        }

        public bool Delete(IEnumerable<int> plannedExecutionIds)
        {
            bool result = true;
            foreach (int id in plannedExecutionIds)
            {
                if (!Delete(id))
                {
                    result = false;
                }
            }
            return result;
        }

        public bool DeleteAll(int reportId)
        {
            string sql = "@DELETE# ReportPlannedExecutions where ReportId= @reportId";
            List<CommandParameter> oParams = new List<CommandParameter>();
            oParams.Add(new CommandParameter("@reportId", CommandParameter.ParameterType.tInt, reportId));

            return AccessHelper.ExecuteSql(sql, oParams);
        }

        #endregion Methods
    }
}