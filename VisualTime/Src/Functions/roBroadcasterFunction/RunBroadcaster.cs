using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Robotics.Base.DTOs;
using Robotics.VTBase;
using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Robotics.Base.VTScheduleManager;

namespace roBroadcasterFunctions
{
    public class RunBroadcaster
    {
        public static MemoryCache memoryCache = MemoryCache.Default;
        private readonly ILogger _logger;

        public RunBroadcaster(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RunBroadcaster>();
        }

        [Function("RunBroadcaster")]
        public void Run([ServiceBusTrigger("broadcastertasks", Connection = "AzureWebJobsServiceBus")] string myQueueItem, ILogger log)
        {
            Robotics.Azure.RoAzureSupport.SetDefaultLogLevel(Program.DefaultLogLevel, Program.DefaultTraceLevel, "roBroadcasterFunction");

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
                        roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "RunBroadcaster::roAzureCache::Could not retrieve company information::" + strCompany);
                    }
                    else
                    {
                        Task.Run(() => ExecuteCompanyAction(oCompanyConfiguration, roTypes.Any2Integer(strTaskId), Program.DefaultLogLevel, Program.DefaultTraceLevel));
                    }
                }
            }
            catch (Exception e)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "RunBroadcaster::" + strTaskId + "::Fatal error::Exception::" + e.Message);
            }
        }

        private static async Task<int> ExecuteCompanyAction(roCompanyConfiguration oCompanyConf, int taskId, int defaultLogLevel, int defaultTraceLevel)
        {
            return await Task.Run(() =>
            {
                if (oCompanyConf != null)
                {
                    bool bInit = Robotics.DataLayer.AccessHelper.SetThreadInformation("roBroadcasterFunction", oCompanyConf, defaultLogLevel, defaultTraceLevel, taskId);

                    if (bInit)
                    {
                        var oTask = new BroadcasterTask();
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