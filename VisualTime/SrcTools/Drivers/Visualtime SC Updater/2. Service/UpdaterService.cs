using SCUpdater;
using System.Diagnostics;
using System.ServiceProcess;

namespace SCServiceUpdater
{
    partial class UpdaterService : ServiceBase
    {
        System.Timers.Timer _timer;
        DateTime _scheduleTime;
        ProxyConfiguration? _proxyConfiguration;

        public UpdaterService(ProxyConfiguration? proxyConfiguration)
        {
            InitializeComponent();
            _timer = new System.Timers.Timer();
            _scheduleTime = DateTime.Today.AddDays(0).AddHours(2); //Ejecutar a las 2am
            _proxyConfiguration = proxyConfiguration;
        }

        protected override void OnStart(string[] args)
        {
            Logger.LogEvent("Visualtime SC Updater Started");
            _timer.Enabled = true;
            double tillNextInterval = _scheduleTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            if (tillNextInterval < 0) tillNextInterval += new TimeSpan(24, 0, 0).TotalSeconds * 1000;
            DateTime executionTime = DateTime.Now.AddMilliseconds(tillNextInterval);

            Logger.LogEvent("SCUpdater: Next execution time: " + executionTime);
            _timer.Interval = tillNextInterval;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
        }

        protected override void OnStop()
        {
            Logger.LogEvent("Visualtime SC Updater stopped");
        }

        protected void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            //Checking for Update
            var updateManager = ServiceUpdater.Instance;
            if(_proxyConfiguration == null)
            {
                Logger.LogEvent("SCUpdater: Couldn't get proxy configuracion from Service");
                return;
            }

            if (updateManager.UpdateService(_proxyConfiguration))
            {
                Logger.LogEvent("SCUpdater: Updated succesfully from Service");
            }
            else 
            {
                Logger.LogEvent("SCUpdater: Not updated from Service");
            }

            //Añadir 24h al timer
            if (_timer.Interval != 24 * 60 * 60 * 1000)
            {
                _timer.Interval = 24 * 60 * 60 * 1000;
            }
        }
    }
}
