using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Robotics.Base.DTOs;
using Robotics.Base.VTScheduleManager;
using Robotics.VTBase;
using roScheduleFunctions;
using System;
using System.Diagnostics;

namespace roSchedulerFunctions
{
    public class RunHourScheduler
    {
        private readonly ILogger _logger;

        public RunHourScheduler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RunHourScheduler>();
        }

        [Function("RunHourTask")]
        public void RunHourTask([TimerTrigger("0 10 * * * *")] TimerInfo myTimer, ILogger log)
        {
            int splitGroupSize = roTypes.Any2Integer(Robotics.VTBase.roConstants.GetConfigurationParameter("SplitGroupSize"));
            if (splitGroupSize <= 0) splitGroupSize = 100;

            Robotics.Azure.RoAzureSupport.SetDefaultLogLevel(Program.DefaultLogLevel, Program.DefaultTraceLevel, "roSchedulerFunctions");
            roConstants.InitializeFunctionCallEnvironment("RunHour");

            if (Robotics.DataLayer.roCacheManager.GetInstance.CheckUpdateCache()) Robotics.DataLayer.roCacheManager.GetInstance.ClearCompaniesCache();
            roCompanyConfiguration[] cachedCompanies = CommonScheduler.CheckIfDBNeedsToUpdate(splitGroupSize);

            if (!CommonScheduler.IsChangeDayRunning())
            {
                try
                {
                    var manager = new roScheduleManager();
                    manager.ChangeHourAction(cachedCompanies, DateTime.Now.Hour, Program.DefaultLogLevel, Program.DefaultTraceLevel, splitGroupSize);
                }
                catch (Exception ex)
                {
                    roLog.get_GetInstance().logMessage(roLog.EventType.roError, "RunHourTask::Error generating company tasks::" + ex.Message);
                }
            }

            roTrace.get_GetInstance().TraceMessage(roTrace.TraceType.roInfo, roTrace.TraceResult.Ok, "Time trigger");
        }
    }
}