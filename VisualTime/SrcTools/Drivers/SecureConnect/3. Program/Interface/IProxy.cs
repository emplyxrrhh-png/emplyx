using System.Threading.Tasks;
using VisualtimeSecureConnect.DTOs;

namespace VisualtimeSecureConnect.Interface
{
    interface IProxy
    {
        Task Start(string remoteServerIp, ushort remoteServerPort, ushort localPort, string localIp = null, ushort latency = 0, ProxyRole role = ProxyRole.Terminal);
    }
}