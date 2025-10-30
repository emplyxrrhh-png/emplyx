using Newtonsoft.Json;
using System;
using System.ServiceProcess;
using System.Text;

namespace SCServiceUpdater
{
    static class Helper
    {

        public const string ENPOINTS_CONFIG_FILE = "endpointsConfig.json";
        public const string PROXY_CONFIG_FILE = "proxyConfig.json";

        public static bool StartService()
        {
            try
            {
                ServiceController thisService = new ServiceController("Visualtime Secure Connect");

                if (thisService.Status == ServiceControllerStatus.Stopped)
                {
                    //Confirmar
                    thisService.Start();
                    thisService.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(1));
                    return true;
                }
            }
            catch (Exception e)
            {

                Logger.LogEvent($"Error starting VisualTime Secure Connect service: {e}");
                return false;
            }
            return false;
        }

        public static bool StopService()
        {
            try
            {
                ServiceController thisService = new ServiceController("Visualtime Secure Connect");

                if (thisService.Status == ServiceControllerStatus.Running)
                {
                    thisService.Stop();
                    thisService.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMinutes(1));
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.LogEvent($"Error stopping VisualTime Secure Connect service: {e}");
                return false;
            }
            return false;
        }

        public static ProxyConfiguration? GetProxyConfiguration()
        {
            try
            {
                string proxyConfigurationJson = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PROXY_CONFIG_FILE));
                ProxyConfiguration? jsonObject = JsonConvert.DeserializeObject<ProxyConfiguration>(proxyConfigurationJson);
                if(jsonObject != null && !string.IsNullOrEmpty(jsonObject.apiUrl))
                {
                    if (!jsonObject.apiUrl.EndsWith("/"))
                    {
                        jsonObject.apiUrl += "/";
                    }

                    jsonObject.updateApiUrl = jsonObject.apiUrl + "CheckVersion";
                    return jsonObject;
                }
            }
            catch (Exception ex)
            {
                Logger.LogEvent($"Exception: Fatal error loading proxy configuration : {ex.Message}");
                return null;
            }
            return null;
        }


        public static bool SaveProxyConfiguration(ProxyConfiguration proxyConfiguration)
        {
            try
            {
                string proxyConfigurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PROXY_CONFIG_FILE);
                File.WriteAllText(proxyConfigurationPath, JsonConvert.SerializeObject(proxyConfiguration, Formatting.Indented));
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogEvent($"Exception: Storing json version : {ex.Message}");
                return false;
            }
        }
    }
}
