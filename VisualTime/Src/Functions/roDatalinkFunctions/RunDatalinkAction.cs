using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Robotics.Base.DTOs;
using Robotics.VTBase;
using System;
using System.Diagnostics;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Robotics.Base.VTScheduleManager;


namespace roDatalinkFunctions
{
    public class RunDatalinkAction
    {
        public static MemoryCache memoryCache = MemoryCache.Default;
        private readonly ILogger _logger;

        public RunDatalinkAction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RunDatalinkAction>();
        }

        [Function("RunDatalinkAction")]
        public void Run([ServiceBusTrigger("datalinktasks", Connection = "AzureWebJobsServiceBus")] string myQueueItem, ILogger log)
        {
            Robotics.Azure.RoAzureSupport.SetDefaultLogLevel(Program.DefaultLogLevel, Program.DefaultTraceLevel, "roDatalinkFunction");

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
                        roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "RunDatalinkAction::roAzureCache::Could not retrieve company information::" + strCompany);
                    }
                    else
                    {
                        Task.Run(() => ExecuteCompanyAction(oCompanyConfiguration, roTypes.Any2Integer(strTaskId), Program.DefaultLogLevel, Program.DefaultTraceLevel));
                    }
                }
            }
            catch (Exception e)
            {
                roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "RunDatalinkAction::" + strTaskId + "::Fatal error::Exception::" + e.Message);
            }
        }

        private static async Task<int> ExecuteCompanyAction(roCompanyConfiguration oCompanyConf, int taskId, int defaultLogLevel, int defaultTraceLevel)
        {
            return await Task.Run(() =>
            {
                if (oCompanyConf != null)
                {
                    bool bInit = Robotics.DataLayer.AccessHelper.SetThreadInformation("roDatalinkFunction", oCompanyConf, defaultLogLevel, defaultTraceLevel, taskId);

                    if (bInit)
                    {
                        var oTask = new DatalinkTask();
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
    }
}