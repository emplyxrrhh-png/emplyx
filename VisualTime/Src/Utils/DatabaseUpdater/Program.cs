using System;
using Robotics.Base.DTOs;
using Robotics.Base.VTScheduleManager;
using Robotics.VTBase;

namespace DatabaseUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("Database Updater - VisualTime");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine();

            try
            {
                Console.WriteLine("Inicializando entorno...");
                
                // Configurar para NO usar Application Insights ni Azure
                System.Configuration.ConfigurationManager.AppSettings["ApplicationInsights.Key"] = "";
                
                // Inicializar el entorno SIN Azure Support
                Console.WriteLine("Configurando DataLayer...");
                Robotics.DataLayer.AccessHelper.InitializeSharedInstanceData(roAppType.roWebApp, roLiveQueueTypes.eog);
                
                Console.WriteLine("Entorno inicializado correctamente");
                
                // Obtener versión actual de la base de datos
                Console.WriteLine();
                Console.WriteLine("Obteniendo versión actual de la base de datos...");
                
                string currentVersion = GetCurrentDBVersion();
                Console.WriteLine($"Versión actual de la base de datos: {currentVersion}");
                
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("? Verificación completada exitosamente");
                Console.ResetColor();
                
                Console.WriteLine();
                Console.WriteLine("NOTA: Para ejecutar actualizaciones de base de datos, es necesario");
                Console.WriteLine("configurar los servicios de Azure (Application Insights, Service Bus, etc.)");
                Console.WriteLine("o ejecutar las actualizaciones manualmente desde SQL Server Management Studio.");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("? Error durante la verificación:");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
                
                Environment.ExitCode = 1;
            }

            Console.WriteLine();
            Console.WriteLine("Presione cualquier tecla para salir...");
            Console.ReadKey();
        }

        private static string GetCurrentDBVersion()
        {
            try
            {
                string query = "SELECT Data FROM sysroParameters WHERE ID = 'DBVERSION'";
                object result = Robotics.DataLayer.AccessHelper.ExecuteScalar(query);
                
                if (result != null)
                {
                    return result.ToString();
                }
                else
                {
                    return "No se pudo obtener la versión";
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Advertencia al obtener versión: {ex.Message}");
                Console.ResetColor();
                return "Error al obtener versión";
            }
        }
    }
}
