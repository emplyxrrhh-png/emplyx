using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Robotics.Azure;
using Robotics.Base.DTOs;
using Robotics.FTP.VTStorageFTPsynchronization;
using Robotics.VTBase;
using roFTPStorageSynchronizationFunction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace roSchedulerFunctions
{
    public class RunSynchronizationScheduler
    {
        private readonly ILogger _logger;

        public RunSynchronizationScheduler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RunSynchronizationScheduler>();
        }

        [Function("RunSynchronizationSchedulerTask")]
        public static void RunSynchronizationSchedulerTask([TimerTrigger("0 0 */1 * * *")] TimerInfo myTimer, ILogger log)
        {
            Robotics.Azure.RoAzureSupport.SetDefaultLogLevel(Program.DefaultLogLevel, Program.DefaultTraceLevel, "roFTPStorageSynchronizationFunction");
            roConstants.InitializeFunctionCallEnvironment("SyncFTPStorage");

            roFTPConnectionsRepository oFTPConnectionsManager = new roFTPConnectionsRepository();

            List<roFTPConnection> oFTPConnections = oFTPConnectionsManager.GetConnections();

            foreach (roFTPConnection oFTPConnection in oFTPConnections)
            {
                roCompanyConfiguration oCompanyConfiguration = Robotics.DataLayer.roCacheManager.GetInstance.GetCompany(oFTPConnection.client);
                Thread.GetDomain().SetData(Thread.CurrentThread.ManagedThreadId.ToString() + "_company", oFTPConnection.client);

                try
                {
                    roTrace.get_GetInstance().AddTraceEvent($"Sync ftp to storage for client {oFTPConnection.client}");
                    ExecuteSynchronizationTask(oCompanyConfiguration, oFTPConnection);
                }
                catch (Exception ex)
                {
                    roLog.get_GetInstance().logMessage(roLog.EventType.roError, "RunSynchronizationSchedulerTask::Error generating company tasks::" + ex.Message);
                }
            }


            roTrace.get_GetInstance().TraceMessage(roTrace.TraceType.roInfo, roTrace.TraceResult.Ok, "Time trigger");
        }

        private static void ExecuteSynchronizationTask(roCompanyConfiguration oCompanyConf, roFTPConnection oFTPConnection)
        {
            if (oCompanyConf != null)
            {
                Dictionary<String, String> o11yDic = roTelemetryInfo.GetInstance.GetO11yInfo();
                Dictionary<String, String> traceDic = roTelemetryInfo.GetInstance.GetTraceInfo();
                roThreadData oThreadData = roConstants.BackupThreadData();

                Robotics.DataLayer.AccessHelper.SetThreadCompanyInformation(oThreadData, o11yDic, traceDic, oCompanyConf);
                
                if (oFTPConnection.action == "import")
                {
                    roStorageFTPsynchronizationImportManager importManager = new roStorageFTPsynchronizationImportManager();
                    importManager.Move2Storage(oFTPConnection);
                }
                else
                {
                    roStorageFTPsynchronizationExportManager exportManager = new roStorageFTPsynchronizationExportManager();
                    exportManager.Move2FTP(oFTPConnection);
                }

                Robotics.DataLayer.AccessHelper.ClearThreadCompanyInformation();
            }
            else
            {
                roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roFTPStorageSynchronizationFunction::ExecuteSynchronizationTask::Fatal error::No client found for request");
            }
        }
    }
}