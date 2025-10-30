using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Robotics.Base.DTOs;
using Robotics.VTBase;

namespace roSenerFunction
{
    public class RunImportScheduler
    {
        private readonly ILogger _logger;
        public RunImportScheduler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RunImportScheduler>();
        }

        [Function("RunImportScheduleTask")]
        public void RunImportScheduleTask([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            Robotics.Azure.RoAzureSupport.SetDefaultLogLevel(Program.DefaultLogLevel, Program.DefaultTraceLevel, "roSenerFunction");

            if (Robotics.DataLayer.roCacheManager.GetInstance.CheckUpdateCache()) Robotics.DataLayer.roCacheManager.GetInstance.ClearCompaniesCache();

            roCompanyConfiguration oCompanyConf = Robotics.DataLayer.roCacheManager.GetInstance.GetCompany(Robotics.VTBase.roConstants.GetConfigurationParameter("ClientName"));

            try
            {
                Task.Run(() => ExecuteCompanyTask(oCompanyConf, Program.DefaultLogLevel, Program.DefaultTraceLevel));
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, "RunExportScheduleTask::Error generating company tasks::" + ex.Message);
            }



        }

        private async Task<int> ExecuteCompanyTask(roCompanyConfiguration oCompanyConf, int defaultLogLevel, int defaultTraceLevel)
        {
            return await Task.Run(() =>
            {
                if (oCompanyConf != null)
                {
                    Robotics.DataLayer.AccessHelper.SetThreadInformation("roSenerFunction", oCompanyConf, defaultLogLevel, defaultTraceLevel, 0);

                    roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roDebug, "roSenerFunction::RunImportScheduleTask:Start Task::Thread " + Thread.CurrentThread.ManagedThreadId.ToString());

                    var importSener = new Robotics.Base.VTSener.roSenerImportTasks();
                    bool ret = importSener.ImportTasks();

                    roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roDebug, "roSenerFunction::RunImportScheduleTask::End Task");
                    Robotics.DataLayer.AccessHelper.ClearThreadInformation();
                }
                else
                {
                    roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roSenerFunction::RunImportScheduleTask::Fatal error::No client found for request");
                }

                return 0;
            }).ConfigureAwait(false);
        }
    }
}
