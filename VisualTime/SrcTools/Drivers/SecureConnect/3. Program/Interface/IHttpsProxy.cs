using NetProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualtimeSecureConnect.DTOs;


namespace VisualtimeSecureConnect.Interface
{
    interface IHttpsProxy
    {
        Task Start(string remoteServerDns, ushort localPort);
    }
}
