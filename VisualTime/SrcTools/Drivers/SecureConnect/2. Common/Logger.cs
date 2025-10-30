using System;
using System.Diagnostics;
using System.IO;
using VisualtimeSecureConnect.DTOs; 

namespace NetProxy
{
    public static class Logger
    {
        private static readonly string _logFilePath = "log.txt";

        public static void Log(string message)
        {
            try
            {
                if (Program.proxyConfiguration != null)
                { 
                switch (Program.proxyConfiguration.logDestination)
                    {
                    case LogDestination.Eventlog:
                        LogEvent(message);
                        break;
                    case LogDestination.File:
                        LogFile(message);
                        break;
                    default:
                            LogEvent(message);
                        break;
                    }
                } else
                {
                    LogEvent(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escribir log: {ex.Message}");
            }
        }

        public static void LogFile(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _logFilePath), true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escribir en el archivo de log: {ex.Message}");
            }
        }

        public static void LogEvent(string message)
        {
            try
            {
                string sourceName = "Visualtime Secure Connect Service";
                string logName = "Application";

                if (!EventLog.SourceExists(sourceName))
                {
                    EventLog.CreateEventSource(sourceName, logName);
                }

                using (EventLog eventLog = new EventLog(logName))
                {
                    eventLog.Source = sourceName;
                    eventLog.WriteEntry(message, EventLogEntryType.Information);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escribir en el archivo de log: {ex.Message}");
            }
        }

    }


}
