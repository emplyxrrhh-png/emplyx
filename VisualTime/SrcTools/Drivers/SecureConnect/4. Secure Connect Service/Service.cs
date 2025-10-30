using System.ServiceProcess;
using System.Threading.Tasks;

namespace NetProxy
{
    public partial class SecureConnectorService : ServiceBase
    {
        public SecureConnectorService()
        {
            InitializeComponent();
        }
        public void OnStart()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            Logger.Log("Visualtime Secure Connect service started");
            Task.Run(() => NetProxy.Program.RunNetProxy());
        }

        protected override void OnStop()
        {
            Logger.Log("Visualtime Secure Connect stopped");
        }
    }
}
