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
    public class RunProgrammedScheduler
    {
        private readonly ILogger _logger;

        public RunProgrammedScheduler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RunProgrammedScheduler>();
        }

        [Function("RunProgrammedTask")]
        public void RunProgrammedTask([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            int splitGroupSize = roTypes.Any2Integer(Robotics.VTBase.roConstants.GetConfigurationParameter("SplitGroupSize"));
            if (splitGroupSize <= 0) splitGroupSize = 100;

            Robotics.Azure.RoAzureSupport.SetDefaultLogLevel(Program.DefaultLogLevel, Program.DefaultTraceLevel, "roSchedulerFunctions");
            roConstants.InitializeFunctionCallEnvironment("RunProgrammed");

            if (Robotics.DataLayer.roCacheManager.GetInstance.CheckUpdateCache()) Robotics.DataLayer.roCacheManager.GetInstance.ClearCompaniesCache();
            roCompanyConfiguration[] cachedCompanies = CommonScheduler.CheckIfDBNeedsToUpdate(splitGroupSize);

            try
            {
                var manager = new roScheduleManager();
                manager.ExecuteProgrammedTasks(cachedCompanies, Program.DefaultLogLevel, Program.DefaultTraceLevel, splitGroupSize);
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "RunProgrammedTask::Error generating company tasks::" + ex.Message);
            }

            roTrace.get_GetInstance().TraceMessage(roTrace.TraceType.roInfo, roTrace.TraceResult.Ok, "Time trigger");

        }

        
    }
}