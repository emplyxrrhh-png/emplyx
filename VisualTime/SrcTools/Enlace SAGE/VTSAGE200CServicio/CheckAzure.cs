using System;
using VTSAGE200CLibrerias;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace VisualTimeSageConnector
{
    public class CheckAzure
    {
        VTSAGE200CLibrerias.Xml ConfigService = new VTSAGE200CLibrerias.Xml();
        VTSAGE200CLibrerias.Logger Log = new VTSAGE200CLibrerias.Logger();
        String versionNuevaAz = "";//String donde se almacena la nueva versión para compararlas
        String versionActualAz = "";//string donde se almacenará la versión del servicio VTSage


        public Boolean ComprobarVersion()//se inicializa el programa
        {
            Boolean Actualizamos= Iniciar();
            return Actualizamos;
        }

        private Boolean Iniciar()
        {
            return VerSiActualizarAZGuard();//leemos los mensajes de las queues con tal de ver si en alguno pone update y actualizar el servicio
            //ActualizarCertificadoSSL.ActualizarCertificado();//habrá que mirar si se tiene que actualizar el certificado de VisualTime. 
           
        }

        //miramos la versión que hay en la tabla de azure de VTSage
        private void LeerVersionTabla()
        {
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            //        CloudConfigurationManager.GetSetting("StorageConnectionString"));
            //CloudTableClient sqlcon = storageAccount.CreateCloudTableClient();
            //CloudTable table = sqlcon.GetTableReference("vtsage200cver");
            //TableOperation retOp = TableOperation.Retrieve<SQLUnidadInstall>("vtsage200c", "version");
            //TableResult resultado = table.Execute(retOp);
            //versionNuevaAz = ((SQLUnidadInstall)resultado.Result).Numero;
        }
        //miramos la versión actual de VTSage
        private void LeerVersion()
        {
            versionActualAz = Assembly.GetEntryAssembly().GetName().Version.ToString();

        }
        //Una vez se ha comprobado que hay una versión nueva ejecutaremos el .exe que será el update
        private void EjecutarUpdate()
        {
            try
            {   //iniciamos el otro archivo. exe
                Log.CrearEntrada("INICIANDO ACTUALIZACIÓN");
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = (new FileInfo(AppDomain.CurrentDomain.BaseDirectory)).Directory.Parent.FullName + @"\Updater\VTSAGE200CUpdater.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Maximized;
                Process.Start(startInfo);
            }
            catch (Exception e)
            {
                Log.CrearEntrada("EROR AL EJECUTAR EL ACTUALIZADOR \n" + e);
            }
        }

        //comprobamos si el servicio está activo o no. LLamamos a la funcion "servicio", y enviamos un correo si está parado. 
        private void Comprobar(string NombreServicio)
        {
            //bool estado = servicio.EstadoServicio(NombreServicio: NombreServicio);
            //if (estado == false)//puede estar parado o stopPending
            //{
            //    bool estadofalse = servicio.servicioFalse(NombreServicio: NombreServicio);
            //    if (estadofalse == false)//está parado el servicio
            //    {
            //        this.estadoDelServicio = "Error";
            //        cont = servicio.estadoStop(cont, estadoDelServicio, NombreServicio);
            //    }
            //    else if (estadofalse == true)//está stopPending el servicio
            //    {
            //        cont = 3;
            //        this.estadoDelServicio = "Ejecutando";
            //        cont = servicio.estadoStop(cont, estadoDelServicio, NombreServicio);
            //    }
            //}
            //else if (estado == true)//está running el servicio
            //{
            //    if (cont == 1)//está running después de una caida
            //    {
            //        cont = 5;
            //        this.estadoDelServicio = "Correcto";
            //        cont = servicio.estadoRun(cont, estadoDelServicio, NombreServicio);
            //    }
            //    else//está running
            //    {
            //        this.estadoDelServicio = "Correcto";
            //        cont = servicio.estadoRun(cont, estadoDelServicio, NombreServicio);
            //    }
            //}
        }

        //Tendremos que ver si la versión que hay en la tabla de azure no corresponde a la que está instalada, si es así se procederá a realizar el update.
        private Boolean VerSiActualizarAZGuard()
        {
            try
            {
                //#### COMPARAMOS LA VERSIÓN ACTUAL CON LA NUEVA ####
                LeerVersionTabla();//miramos la versión que hay en la tabla
                LeerVersion();//leemos la versión que hay intalada
                //comparamos un string con otro
                if (!(versionActualAz == versionNuevaAz))
                {
                    Log.CrearEntrada("Versión actual:" + versionActualAz);
                    Log.CrearEntrada("Versión de la tabla azure:" + versionNuevaAz);
                    Log.CrearEntrada("ACTUALIZACIÓN DE VTSage200");
                    try
                    {
                        EjecutarUpdate();//ejecutamos el update
                        return true;
                        //cambiarVersionTXT(versionNuevaAz);//se cambiará por el update, ya que de esta forma nos aseguramos que se ha actualizado correctamente
                    }
                    catch (Exception e)
                    {
                        Log.CrearEntrada("No se ha podido Actualizar el Servicio " + e);
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Log.CrearEntrada("ERROR AL LEER LA VERSIÓN: " + e);
                return false;
            }
        }
    }
}
