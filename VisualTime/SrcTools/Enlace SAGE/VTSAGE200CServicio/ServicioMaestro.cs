using System;
using System.Data;
using System.ServiceProcess;
using System.Timers;
using System.Reflection;
using VTSAGE200CLibrerias;

namespace VisualTimeSageConnector
{
    public partial class ServicioMaestro : ServiceBase
    {
        static bool debug = true; // Si esta activo muestra Informacion Extra.
        System.Timers.Timer DuracionMinutos;//tiempo que estará entre comprobación y comprobación
        System.Timers.Timer ExportAccruals;//tiempo que estará entre comprobación y comprobación
        //ftp ftpClient = new ftp();
        //RegeditVT regedit = new RegeditVT();
        //directory Directorio = new directory();
        Xml ConfigService = new VTSAGE200CLibrerias.Xml();
        Params Params = new VTSAGE200CLibrerias.Params();
        WS WService = new WS();
        WSMT WServiceMT = new WSMT();
        private VTSAGE200CLibrerias.Logger Logger = new VTSAGE200CLibrerias.Logger();
        ConexionSage cnnSage = new ConexionSage();
        public ServicioMaestro()
        {
            InitializeComponent(); 
        }

        public void OnStart()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            Logger.CrearEntrada("Bienvenido al enlace VisualTime Live - SAGE 200c * Versión " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            
            //Gestión Importar Empleados modificados
            Tiempo();

            //Espero que el servicio acabe de iniciar
            System.Threading.Thread.Sleep(5000);

            //Nada más arrancar ejecuto por primera vez ... Luego cuando toque según el timer
            Comparar();

            // Gestión Exportacion de Saldos
            if (Params.Accrual.SelectSingleNode("Active").InnerText == "1")
            {
                ExportAccruals = new System.Timers.Timer();
                ExportAccruals.Elapsed += new ElapsedEventHandler(TickAccrual);
                ExportAccruals.Interval = (Convert.ToDouble(Params.Accrual.SelectSingleNode("Frequency").InnerText) * (60 * 1000)); //24horas * 60 = minutos, *60 = segundos, *1000 = milisegundos, * Frecuency = días en milisegundos                
                ExportAccruals.Enabled = true;
            }
            else if (Params.Accrual.SelectSingleNode("Active").InnerText == "2")
            {
            }
            else if (Params.Accrual.SelectSingleNode("Active").InnerText == "3")
            {
                // SI 
                //si Active = 3, lanzaremos la exportación de saldos una sola vez al iniciar el servicio, se crea esta parametrización para un forzado de sincronismo de saldos por lo que pueda ser necesario 
                CompararAccrual();
                //ExportAccruals.Enabled = false;
            }
        }
        //Apartado que controla el tiempo que tardará en recorrer de nuevo el servicio.
        private void Tiempo()
        {
            //Timer para la importación de empleados
            DuracionMinutos = new System.Timers.Timer();
            DuracionMinutos.Elapsed += new ElapsedEventHandler(Tick);//por cada vuelta que se reinicia ejecutará esa función.
            DuracionMinutos.Interval = (ConfigService.Timer * 60) * 1000 ;
            DuracionMinutos.Enabled = true;
          }



        //Haremos que por cada vuelta de comprobación ejecuten las siguientes funciones:
        private void Tick(object source, ElapsedEventArgs e)
        {
            if (debug)
            {
                //  Logger.CrearEntrada("Hace Tick");
            }
            DuracionMinutos.Enabled = false;
            Comparar();

        }
        private void TickAccrual(object source, ElapsedEventArgs e)
        {
            if (debug)
            {
                //  Logger.CrearEntrada("Hace Tick");
            }
            CompararAccrual();

        }
        private void Comparar()
        {
             //Logger.CrearEntrada(ConfigService.WebService +" " + ConfigService.WSPassword);

            try
            {
                //Revisar versión para lanzar o no el actualizador
                //CheckAzure CheckVersion = new CheckAzure();
                //Boolean Actualizamos = CheckVersion.ComprobarVersion();
                //if (Actualizamos == false)
                //{
                    //Exportar empleados de Sage a VisualTime
                    cnnSage.ObtenerEmpleado();

                    
                    //Importar Accruals REAL
                    //DataTable Empleados = cnnSage.ObtenerEmpleados("");
                    //WService.ObtenerAccrualsEmpleados(Empleados);
                //}

            }
            catch (Exception Ex) {
                  Logger.CrearEntrada("ServicioMaster, catchComparar: " +Ex.Message);
            }
              //Logger.CrearEntrada("pasado el TC");


            DuracionMinutos.Enabled = true;
        }
        private void CompararAccrual()
        {
            //Logger.CrearEntrada(ConfigService.WebService +" " + ConfigService.WSPassword);

            try
            {
                //Revisar versión para lanzar o no el actualizador
                //CheckAzure CheckVersion = new CheckAzure();
                //Boolean Actualizamos = CheckVersion.ComprobarVersion();
                //if (Actualizamos == false)
                //{

                //Importar Accruals REAL
                //Logger.CrearEntrada("Export Accruals " + Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy")).AddDays(Convert.ToDouble(Params.Accrual.SelectSingleNode("days").InnerText) * -1).ToString());
                Logger.CrearEntrada("Export Accruals " + Convert.ToDateTime(DateTime.Now.Date.AddDays(Convert.ToDouble(Params.Accrual.SelectSingleNode("days").InnerText) * -1).ToString()));
                DataTable Empleados = cnnSage.ObtenerEmpleados("");
                if (ConfigService.WSMT)
                {
                    WServiceMT.ObtenerAccrualsEmpleados(Empleados);
                }
                else
                {
                    WService.ObtenerAccrualsEmpleados(Empleados);
                }
                    
                //}

            }
            catch (Exception Ex)
            {
                Logger.CrearEntrada("ServicioMaster, catchCompararAccrual: " + Ex.Message);
            }
            //Logger.CrearEntrada("pasado el TC");

        }
        //
        // Resumen:
        //     Cuando se implementa en una clase derivada, System.ServiceProcess.ServiceBase.OnContinue
        //     se ejecuta cuando se envía un comando Continuar al servicio mediante el Administrador
        //     de Control de servicios (SCM). Especifica las acciones que deben realizarse cuando
        //     un servicio reanuda el funcionamiento normal después de una pausa.
        protected override void OnContinue()
        {
            Tiempo();
        }

        //
        // Resumen:
        //     Cuando se implementa en una clase derivada, se ejecuta cuando se envía un comando
        //     Pausar al servicio mediante el Administrador de Control de servicios (SCM). Especifica
        //     las acciones que deben realizarse cuando un servicio se detiene.
        protected override void OnPause()
        {
            DuracionMinutos.Enabled = false;
            ExportAccruals.Enabled = false;
        }


        //
        // Resumen:
        //     Cuando se implementa en una clase derivada, se ejecuta cuando se apaga el sistema.
        //     Especifica qué debe ocurrir justo antes del cierre del sistema.
        protected override void OnShutdown()
        {
             Logger.CrearEntrada("Deteniendo servicio conector VisualTime Live - SAGE 200c por parada de servidor");
        }

        protected override void OnStop()
        {
              Logger.CrearEntrada("Servicio conector VisualTime Live - SAGE 200c detenido");
        }



        // DESCOMENTAR LAS DOS FUNCIONES SIGUIENTES PARA PRUEBAS EN LOCAL. ADEMÁS, CAMBIAR EN PROPIEDADES EL PROYECTO DE INICIO
        //internal void TestStartupAndStop(string[] args)
        //{
        //    this.OnStart(args);
        //    while (true)
        //    {
        //        System.Threading.Thread.Sleep(2000);
        //    }
        //}
        //static void Main(string[] args)
        //{
        //    if (Environment.UserInteractive)
        //    {
        //        ServicioMaestro service1 = new ServicioMaestro();
        //        service1.TestStartupAndStop(args);
        //    }
        //    else
        //    {
        //        // Put the body of your old Main method here.  
        //    }
        //}
    }
}
