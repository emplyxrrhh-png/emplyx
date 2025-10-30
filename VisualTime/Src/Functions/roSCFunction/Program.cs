using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Robotics.Base.DTOs;
using Robotics.VTBase;

namespace roSCFunctions
{
    internal class Program
    {
        public static int DefaultLogLevel = 0;
        public static int DefaultTraceLevel = 0;
        private static void Main(string[] args)
        {
            FunctionsDebugger.Enable();

            Robotics.DataLayer.AccessHelper.InitializeSharedInstanceData(roAppType.roSCFunction, roLiveQueueTypes.pnlink);
            Program.DefaultLogLevel = roTypes.Any2Integer(System.Threading.Thread.GetDomain().GetData(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_DefaultLogLevel"));
            Program.DefaultTraceLevel = roTypes.Any2Integer(System.Threading.Thread.GetDomain().GetData(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_DefaultTraceLevel"));

            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .Build();

            host.Run();
        }
    }
}