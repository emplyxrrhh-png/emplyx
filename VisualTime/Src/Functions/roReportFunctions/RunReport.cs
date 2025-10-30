using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ReportGenerator.Repositories;
using ReportGenerator.Services;
using ReportGenerator.TaskManager;
using Robotics.Base.DTOs;
using Robotics.Base.VTScheduleManager;
using Robotics.VTBase;
using System;
using System.Diagnostics;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace roReportFunctions
{
    public class RunReport
    {
        public static MemoryCache memoryCache = MemoryCache.Default;
        private readonly ILogger _logger;

        public RunReport(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RunReport>();
        }

        [Function("RunReport")]
        public void Run([ServiceBusTrigger("reporttasks", Connection = "AzureWebJobsServiceBus")] string myQueueItem, ILogger log)
        {
            Robotics.Azure.RoAzureSupport.SetDefaultLogLevel(Program.DefaultLogLevel, Program.DefaultTraceLevel, "roReportFunctions");

            var strTaskId = string.Empty;
            var strCompany = string.Empty;

            try
            {
                string body = myQueueItem;

                strCompany = body.Split('@')[0];
                strTaskId = body.Split('@')[1];

                if (strTaskId == "-999") Robotics.DataLayer.roCacheManager.GetInstance.ClearCompaniesCache();

                roCompanyConfiguration oCompanyConfiguration = Robotics.DataLayer.roCacheManager.GetInstance.GetCompany(strCompany);

                if (strTaskId != "-999")
                {
                    if (oCompanyConfiguration == null)
                    {
                        roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "RunReport::Could not retrieve company information::" + strCompany);
                    }
                    else
                    {
                        Task.Run(() => ExecuteCompanyAction(oCompanyConfiguration, roTypes.Any2Integer(strTaskId), Program.DefaultLogLevel, Program.DefaultTraceLevel));
                    }
                }
            }
            catch (Exception e)
            {
                roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "RunReport::Fatal error::Exception::" + e.Message);
            }
        }

        private static async Task<int> ExecuteCompanyAction(roCompanyConfiguration oCompanyConf, int taskId, int defaultLogLevel, int defaultTraceLevel)
        {
            return await Task.Run(() =>
            {
                if (oCompanyConf != null)
                {
                    bool bInit = Robotics.DataLayer.AccessHelper.SetThreadInformation("roReportFunctions", oCompanyConf, defaultLogLevel, defaultTraceLevel, taskId);

                    if (bInit)
                    {
                        var reportGeneratorService = CreateReportGenerator();
                        var ReportStorageService = CreateReportStorage(oCompanyConf.Id);
                        var ReportExecutionService = new ReportExecutionService();

                        var oTask = new ReportTaskManager(ReportExecutionService, ReportStorageService, reportGeneratorService);
                        oTask.ExecuteTask(taskId);
                    }
                    else
                    {
                        roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roDebug, $"Task delayed due to init");
                    }

                    Robotics.DataLayer.AccessHelper.ClearThreadInformation();
                }
                else
                {
                    roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, $"Task {taskId}::Fatal error::No client found for request");
                }

                return 0;
            }).ConfigureAwait(false);
        }

        private static ReportGeneratorService CreateReportGenerator()
        {
            return new ReportGeneratorService();
        }

        private static ReportStorageService CreateReportStorage(String strCompanyId)
        {
            var blobRepo = new BlobRepository(strCompanyId);
            var executionRepo = new ReportExecutionRepository();

            return new ReportStorageService(blobRepo, executionRepo);
        }
    }
}