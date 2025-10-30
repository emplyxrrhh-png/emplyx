using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using VisualtimeSecureConnect.DTOs;
using System.Text.Json.Nodes;

namespace NetProxy
{
    static class Helper
    {

        public const string ENPOINTS_CONFIG_FILE = "endpointsConfig.json";
        public const string PROXY_CONFIG_FILE = "proxyConfig.json";

        private static void StartService()
        {
            ServiceController thisService = new ServiceController("Visualtime Secure Connect");

            if (thisService.Status == ServiceControllerStatus.Running)
            {
                //Confirmar
                thisService.Start();
                thisService.WaitForStatus(ServiceControllerStatus.Running);
            }
        }

        private static void StopService()
        {
            ServiceController thisService = new ServiceController("Visualtime Secure Connect");

            if (thisService.Status == ServiceControllerStatus.Running)
            {
                thisService.Stop();
                thisService.WaitForStatus(ServiceControllerStatus.Stopped);
            }
        }

        public static ProxyConfiguration GetProxyConfiguration()
        {
            try
            {
                string proxyConfigurationJson = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PROXY_CONFIG_FILE));
                return JsonConvert.DeserializeObject<ProxyConfiguration>(proxyConfigurationJson); 
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception: Fatal error loading proxy configuration : {ex.Message}");
                return null;
            }
        }

        public static string GetEndpointsConfiguration(ProxyConfiguration proxyConfiguration)
        {
            try
            {
                HttpClient configClient = new HttpClient();
                configClient.Timeout = new TimeSpan(0, 0, 100);
                var jsonApiURL = proxyConfiguration.apiUrl;
                if (!jsonApiURL.EndsWith("/"))
                {
                    jsonApiURL += "/";
                }

                jsonApiURL += "endpoint";
                var response = configClient.GetAsync($"{jsonApiURL}/{proxyConfiguration.customerId}/{proxyConfiguration.apiToken}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    // Doy formato para que sea más legible
                    JObject json = JObject.Parse(result);
                    result = JsonConvert.SerializeObject(json, Formatting.Indented);
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception: Fatal error loading endpoints configuration: {ex.Message}\n{ex.InnerException}");
                return null;
            }
        }

        public static string EndPointsRaw(EndPointsConfiguration config)
        {
            StringBuilder result = new StringBuilder();

            try
            {
                foreach (var kvp in config.endPoints)
                {
                    VisualtimeSecureConnect.DTOs.EndPoint endPoint = kvp.Value;

                    result.Append($"{kvp.Key}:localPort={endPoint.localPort},localIp={endPoint.localIp},forwardIp={endPoint.forwardIp},forwardPort={endPoint.forwardPort},latency={endPoint.latency};");
                }
            }
            catch (Exception) { }

            return result.ToString();
        }

    }
}
