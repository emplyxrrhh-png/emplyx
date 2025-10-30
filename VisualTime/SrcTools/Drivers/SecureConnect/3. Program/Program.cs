using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.ServiceProcess;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading;
using VisualtimeSecureConnect.DTOs;
using Microsoft.Win32;

namespace NetProxy
{

    static class Program
    {
        private static EndPointsConfiguration _endPointsConfiguration = null;
        private static ProxyConfiguration _proxyConfiguration = null;
        private static DiagnosticsInfo _diagnosticsInfo = null;
        private static Timer timer;

        public static EndPointsConfiguration endPointsConfiguration { get => _endPointsConfiguration; set => _endPointsConfiguration = value; }
        public static ProxyConfiguration proxyConfiguration { get => _proxyConfiguration; set => _proxyConfiguration = value; }
        public static DiagnosticsInfo diagnosticsInfo { get => _diagnosticsInfo; set => _diagnosticsInfo = value; }

        static void Main()
        {
            string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Robotics\\VisualTime\\Server";
            string valueName = "IsPod";
            object value = Registry.GetValue(keyName, valueName, null);

            if (Environment.UserInteractive || (value != null && int.TryParse(value.ToString(), out int registryValue) && registryValue == 1))
            {
                // Ejecuta el servicio de forma interactiva para depuración
                Logger.Log("Visualtime Secure Connect starting in interactive mode ...");
                RunNetProxy();
            }
            else
            {
                try
                {
                    Logger.Log("Visualtime Secure Connect starting in service mode ...");
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                        new SecureConnectorService()
                    };
                    ServiceBase.Run(ServicesToRun);
                }
                catch (Exception ex)
                {
                    Logger.Log("Exception: Could not start service: " + ex.ToString());
                }
            }
        }

        public static void RunNetProxy()
        {
            try
            {
                //Cargamos configuración llamando a la API de configuración
                Program.proxyConfiguration = Helper.GetProxyConfiguration();
                if (Program.proxyConfiguration == null)
                {
                    //Descargamos
                    return;
                }

                //Iniciamos el servicio de diagnóstico si así se configuró
                Program.diagnosticsInfo = new DiagnosticsInfo();                
                if (Program.proxyConfiguration.diagnosticsPort != null && Program.proxyConfiguration.diagnosticsPort.Length > 0) {
                    Program.diagnosticsInfo.endpointsConfigSource = "api";
                    Program.diagnosticsInfo.status = ProxyStatus.Stopped;
                    Program.diagnosticsInfo.customerId = Program.proxyConfiguration.customerId;
                    Program.diagnosticsInfo.role = Program.proxyConfiguration.role.ToString();
                    Task.Run(() => Diagnostics.RunDiagnostics());
                }

                //Cargamos la configuración de los endpoints
                string endpointConfigurationJson = null;
                switch (Program.proxyConfiguration.role)
                {
                    case ProxyRole.Terminal:
                        endpointConfigurationJson = Helper.GetEndpointsConfiguration(Program.proxyConfiguration);

                        if (endpointConfigurationJson == null || endpointConfigurationJson.Length == 0)
                        {
                            Logger.Log("Could not load dynamic proxy endpoints configuration from API ...");
                            //Si no se carga la configuración desde la API, miro si debo cargarla desde fichero (emergency mode)
                            if (Program.proxyConfiguration.emergency)
                            {
                                Logger.Log("Entering emergency mode. Loading config from local file ...");
                                endpointConfigurationJson = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.ENPOINTS_CONFIG_FILE));
                                Program.diagnosticsInfo.endpointsConfigSource = "local";
                            }
                            else
                            {
                                Logger.Log("Bye !!!");
                                Program.diagnosticsInfo.status = ProxyStatus.Stopped;
                                return;
                            }
                        }
                        else
                        {
                            //Si se ha cargado la configuración desde la API, la guardo en fichero para que esté disponible en modo emergency
                            Logger.Log($"Endpoints configuration loaded from {Program.proxyConfiguration.apiUrl}endpoint");
                            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.ENPOINTS_CONFIG_FILE), endpointConfigurationJson);
                        }
                        break;
                    case ProxyRole.Server:
                        endpointConfigurationJson = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.ENPOINTS_CONFIG_FILE));
                        break;
                }

                endPointsConfiguration = JsonConvert.DeserializeObject<EndPointsConfiguration>(endpointConfigurationJson);
                foreach (var endpoint in endPointsConfiguration.endPoints)
                {
                    if (!Program.diagnosticsInfo.forwardIp.Contains(endpoint.Value.forwardIp))
                    {
                        Program.diagnosticsInfo.forwardIp += endpoint.Value.forwardIp + "<br>";
                    }
                }

                Logger.Log($"Start applying endpoint configuration loaded {endpointConfigurationJson}");

                // Para proxy cliente, revisión de la configuración de los endpoints cada 24 horas, empezando 1 hora después de la carga inicial
                Program.diagnosticsInfo.checkEndpointsEvery = 0;
                if (Program.proxyConfiguration.role == ProxyRole.Terminal)
                {
                    int checkEvery = Program.proxyConfiguration.checkEndpointsEvery > 0 ? Program.proxyConfiguration.checkEndpointsEvery : 24 * 60;
                    timer = new Timer(CheckProxyConfiguration, null, checkEvery * 60 * 1000, checkEvery * 60 * 1000);
                    Program.diagnosticsInfo.checkEndpointsEvery = checkEvery;
                }

                Program.diagnosticsInfo.status = ProxyStatus.Running;
                Task.WhenAll(endPointsConfiguration.endPoints.Select((Func<KeyValuePair<string, VisualtimeSecureConnect.DTOs.EndPoint>, Task>)(c =>
                {
                    try
                    {
                        switch (c.Value.proxyType)
                        {
                            case ProxyType.tcp:
                                var tcpProxy = new TcpProxy();
                                return tcpProxy.Start(c.Value.forwardIp, c.Value.forwardPort, c.Value.localPort, c.Value.localIp, c.Value.latency, Program.proxyConfiguration.role);
                            case ProxyType.https:
                                var httpsProxy = new HttpsProxy();
                                return httpsProxy.Start(c.Value.forwardIp, c.Value.localPort);
                        }
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Exception: Could not start {c.Key} endpoint: {ex.Message}");
                        throw;
                    }
                })
                )).Wait();
            }
            catch (Exception ex)
            {
                Program.diagnosticsInfo.status = ProxyStatus.Stopped;
                Logger.Log($"Exception: Fatal error starting proxy: {ex.Message}");
            }
        }

        private static void CheckProxyConfiguration(object state)
        {
            try
            {
                string newEndpointConfigurationJson = Helper.GetEndpointsConfiguration(Program.proxyConfiguration);
                EndPointsConfiguration newEndPointsConfiguration = JsonConvert.DeserializeObject<EndPointsConfiguration>(newEndpointConfigurationJson);
                if (Helper.EndPointsRaw(newEndPointsConfiguration) != Helper.EndPointsRaw(endPointsConfiguration))
                {
                    Logger.Log($"New endpoints configuration detected from {Program.proxyConfiguration.apiUrl}endpoint. Service will be restarted to reload configs.");
                    Environment.Exit(0);
                }
                else
                {
                    Logger.Log($"No changes detected in endpoints configuration from {Program.proxyConfiguration.apiUrl}endpoint.");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error checking proxy configuration (assuming no changes): {ex.Message}");
            }
        }
    }
}