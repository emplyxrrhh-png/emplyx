// See https://aka.ms/new-console-template for more information
using SCServiceUpdater;
using SCUpdater;
using System.ServiceProcess;

public static class Program
{
    public static void Main(string[] args)
    {
        var updateManager = ServiceUpdater.Instance;
        var configuration = Helper.GetProxyConfiguration();
        if(configuration == null) 
        {
            Logger.LogEvent("SCUpdater: Could not get configuration");
            return;
        }

        if (Environment.UserInteractive)
        {
            if (updateManager.UpdateService(configuration))
            {
                Logger.LogEvent("SCUpdater: Updated succesfully from User Interactive");
            }
            else
            {
                Logger.LogEvent("SCUpdater: Not updated from User Interactive");
            }
        }
        else
        {
            try
            {
                Logger.LogEvent("SCUpdater: Starting in service mode ...");
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                        new UpdaterService(configuration)
                };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                Logger.LogEvent("Exception: Could not start service: " + ex.ToString());
            }
        }
    }
}