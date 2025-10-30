using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Robotics.Base.DTOs;
using Robotics.VTBase;
using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Robotics.Base.VTScheduleManager;

namespace roBackgroundFunctions
{
    public class RunBackgroundTask
    {
        public static MemoryCache memoryCache = MemoryCache.Default;
        private readonly ILogger _logger;

        public RunBackgroundTask(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RunBackgroundTask>();
        }

        [Function("RunBackgroundTask")]
        public void Run([ServiceBusTrigger("eogtasks", Connection = "AzureWebJobsServiceBus")] string myQueueItem, ILogger log)
        {
            Robotics.Azure.RoAzureSupport.SetDefaultLogLevel(Program.DefaultLogLevel, Program.DefaultTraceLevel, "roBackgroundFunction");

            var strTaskId = string.Empty;
            var strCompany = string.Empty;
            try
            {
                string body = myQueueItem;

                string[] reqPrams = body.Split('@');
                strCompany = reqPrams[0];
                strTaskId = reqPrams[1];
                var strTaskAction = (reqPrams.Length == 3) ? body.Split('@')[2] : "";

                if (strTaskId == "-999") Robotics.DataLayer.roCacheManager.GetInstance.ClearCompaniesCache();

                roCompanyConfiguration oCompanyConfiguration = Robotics.DataLayer.roCacheManager.GetInstance.GetCompany(strCompany);

                if (strTaskId != "-999")
                {
                    if (oCompanyConfiguration == null)
                    {
                        roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "RunBackgroundTask::roAzureCache::Could not retrieve company information::" + strCompany);
                    }
                    else
                    {
                        Task.Run(() => ExecuteCompanyAction(oCompanyConfiguration, roTypes.Any2Integer(strTaskId), strTaskAction,Program.DefaultLogLevel, Program.DefaultTraceLevel));
                    }
                }
            }
            catch (Exception e)
            {
                roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "RunBackgroundTask::" + strTaskId + "::Fatal error::Exception::" + e.Message);
            }
        }

        private static async Task<int> ExecuteCompanyAction(roCompanyConfiguration oCompanyConf, int taskId, String action, int defaultLogLevel, int defaultTraceLevel)
        {
            return await Task.Run(() =>
            {
                if (oCompanyConf != null)
                {
                    bool bInit = Robotics.DataLayer.AccessHelper.SetThreadInformation("roBackgroundFunction", oCompanyConf, defaultLogLevel, defaultTraceLevel, taskId, action);

                    if (bInit)
                    {
                        var oTask = new EOGServerTask();
                        oTask.ExecuteTask(taskId, action);
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
    }
}