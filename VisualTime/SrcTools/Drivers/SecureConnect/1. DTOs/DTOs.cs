using NetProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualtimeSecureConnect.DTOs
{
    public enum ProxyRole
    {
        Terminal,
        Server
    }

    public enum ProxyStatus
    {
        Stopped,
        Running
    }

    public enum ProxyType
    {
        tcp,
        https
    }

    public enum LogDestination
    {
        Eventlog,
        File
    }

    public class ProxyConfiguration
    {
        public ProxyRole role { get; set; } = ProxyRole.Terminal;

        public string apiUrl { get; set; } = "";

        public string apiToken { get; set; } = "";

        public string customerId { get; set; } = "";

        public bool emergency { get; set; } = false;

        public string diagnosticsPort { get; set; } = "";

        public int checkEndpointsEvery { get; set; } = 0;

        public string version { get; set; } = "";

        public LogDestination logDestination { get; set; } = LogDestination.Eventlog;
    }

    public class EndPointsConfiguration
    {
        public Dictionary<string, EndPoint> endPoints { get; set; }
    }

    public class EndPoint
    {
        public ushort localPort { get; set; } = 0;
        public string localIp { get; set; } = "";
        public string forwardIp { get; set; } = "";
        public ushort forwardPort { get; set; } = 0;
        public ushort latency { get; set; } = 0;
        public ProxyType proxyType { get; set; } = ProxyType.tcp;
    }

    public class DiagnosticsInfo
    {
        public ProxyStatus status { get; set; } = ProxyStatus.Stopped;
        public string customerId { get; set; } = "";
        public string role { get; set; } = "";
        public string forwardIp { get; set; } = "";
        public string apiUrl { get; set; } = "";
        public string emergency { get; set; } = "";
        public int checkEndpointsEvery { get; set; } = 0;
        public string endpointsConfigSource { get; set; } = "";

    }
}
