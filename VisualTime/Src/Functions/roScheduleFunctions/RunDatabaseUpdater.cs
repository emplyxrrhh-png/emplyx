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
    public class RunDatabaseUpdater
    {
        private readonly ILogger _logger;

        public RunDatabaseUpdater(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RunDatabaseUpdater>();
        }

        [Function("RunDatabaseUpdaterTask")]
        public void RunDatabaseUpdaterTask([TimerTrigger("0 20 * * * *")] TimerInfo myTimer, ILogger log) //Cron una vez cada hora en el minuto 20
        {
            int splitGroupSize = roTypes.Any2Integer(Robotics.VTBase.roConstants.GetConfigurationParameter("SplitGroupSize"));
            if(splitGroupSize <= 0) splitGroupSize = 100;

            Robotics.Azure.RoAzureSupport.SetDefaultLogLevel(Program.DefaultLogLevel, Program.DefaultTraceLevel, "roSchedulerFunctions");
            roConstants.InitializeFunctionCallEnvironment("RunDatabaseUpdater");

            if (Robotics.DataLayer.roCacheManager.GetInstance.CheckUpdateCache()) Robotics.DataLayer.roCacheManager.GetInstance.ClearCompaniesCache();
            roCompanyConfiguration[] cachedCompanies = CommonScheduler.CheckIfDBNeedsToUpdate(splitGroupSize);

            try
            {
                var manager = new roScheduleManager();
                manager.UpdateDBTasks(cachedCompanies, Program.DefaultLogLevel, Program.DefaultTraceLevel, splitGroupSize);
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "RunDatabaseUpdaterTask::Error checking for DB version::" + ex.Message);
            }

            roTrace.get_GetInstance().TraceMessage(roTrace.TraceType.roInfo, roTrace.TraceResult.Ok, "Time trigger");
        }
    }
}