using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading;
using Microsoft.WindowsAzure.Storage.Auth;

namespace SaaSPrintingTest
{
    internal class Service
    {
        public Service()
        {
            Start(null);
        }

        public void WriteLog(string log)
        {
            try
            {
                File.AppendAllLines(ConfigurationManager.AppSettings["LogPath"] + DateTime.Today.ToString("yyyy_MM_dd") + ".log", new string[] { log });
            }
            catch { }
        }

        protected void Start(string[] args)

        {
            string Behaviour = ConfigurationManager.AppSettings["Behaviour"];


            switch (Behaviour)
            {
                case "Client":
                    ClientBehaviour();
                    break;
                case "Server":
                    WriteLog("Esta versión de SaaS printing service no soporta el comportamiento 'Server'"); ;
                    break;
            }

        }

        protected void ClientBehaviour()
        {
            WriteLog("Inicio del Servicio en Cliente: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));

            String LocalPath = ConfigurationManager.AppSettings["ClientPath"];

            try
            {
                FileSystemWatcher fw = new FileSystemWatcher();
                fw.Path = @LocalPath;
                fw.IncludeSubdirectories = true;
                fw.Filter = "*.*";
                fw.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                    | NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.FileName
                                    | NotifyFilters.Security | NotifyFilters.Size;

                fw.Created += cw_DetectedOnClient;
                fw.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                WriteLog("Servicio detenido: " + ex.Message + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
            }

            string Blob = ConfigurationManager.AppSettings["Blob"];
            string Container = ConfigurationManager.AppSettings["Container"];


            bool isMTKey = ConfigurationManager.AppSettings.AllKeys.ToList().Find(x => x == "IsMT") != null ? bool.Parse(ConfigurationManager.AppSettings["IsMT"].ToLower()) : false;



            CloudStorageAccount storageAccount = null;
            CloudBlobContainer container = null;
            String folder = String.Empty;
            if (!isMTKey)
            {
                WriteLog("Inicio entorno SaaS/OnPrem: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
                storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                WriteLog("... Conectando al contender: " + Container.ToLower() + " (" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt") + ")");
                container = blobClient.GetContainerReference(Container.ToLower());
                container.CreateIfNotExists();
            }
            else
            {
                folder = "diner/";
                WriteLog("Inicio entorno MT: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
                StorageCredentials accountSAS = new StorageCredentials(ConfigurationManager.AppSettings["StorageConnectionString"]);

                storageAccount = new CloudStorageAccount(accountSAS, "romtidi01storage", "", true);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                container = blobClient.GetContainerReference(Container.ToLower());
            }

            if (container != null)
            {
                WriteLog("... Container de cliente verificado y correcto. (" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt") + ")");

                var BlobSyncro = new BlobSync(container, LocalPath, "diner", TimeSpan.FromSeconds(1));

                BlobSyncro.SyncAll();
                //string pattern = folder + "DinnerPunchInfo";
                //var lBlobs = container.ListBlobs(pattern);


                //foreach (CloudBlockBlob oBlob in lBlobs)
                //{
                //    WriteLog($"{oBlob.Name} ");
                //}
            }
            else
            {
                WriteLog("... No se ha podido obtener la referencia del contenedor. (" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt") + ")");
            }
        }

        //protected void ServerBehaviour()
        //{
        //    WriteLog("Inicio del Servicio en Servidor: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
        //    string Path = ConfigurationManager.AppSettings["ServerPath"];

        //    System.IO.DirectoryInfo di = new DirectoryInfo(Path);

        //    foreach (FileInfo file in di.GetFiles())
        //    {
        //        WriteLog(file.Extension);

        //        if ((file.CreationTimeUtc < (DateTime.UtcNow - new TimeSpan(0, 5, 0))) || (file.Extension != ".txt"))
        //        {
        //            file.Delete();
        //            WriteLog("Se ha eliminado por caducidad o formato incorrecto el archivo: " + file.Name + " " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
        //        }
        //        else
        //        {
        //            CheckLock(file.FullName);
        //            WriteLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff tt") + " Archivo atrasado detectado en el servidor= " + file.Name +  " ");
        //        }

        //    }

        //    try
        //    {

        //        FileSystemWatcher fw = new FileSystemWatcher();
        //        fw.Path = @Path;
        //        fw.IncludeSubdirectories = true;
        //        fw.Filter = "*.*";
        //        fw.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite
        //                            | NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.FileName
        //                            | NotifyFilters.Security | NotifyFilters.Size;

        //        fw.Created += fw_CreatedOnServer;
        //        fw.EnableRaisingEvents = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog("Servicio detenido: " + ex.Message + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
        //    }

        //}

        void cw_DetectedOnClient(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.Contains("FsLog"))
            {
                CheckLock(e.FullPath);
                WriteLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff tt") + " Archivo detectado desde el cliente= " + e.ChangeType + " Ruta = " + e.FullPath + " ");
            }
        }

        //void fw_CreatedOnServer(object sender, FileSystemEventArgs e)
        //{
        //    if (!e.FullPath.Contains("FsLog"))
        //    {
        //        CheckLock(e.FullPath);
        //        WriteLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff tt") + " Archivo creado en el servidor= " + e.ChangeType + " Ruta = " + e.FullPath + " ");
        //    }
        //}

        //protected void UploadTicket(string contenedor, string blobName , string Path)
        //{

        //    string contenedorLower = contenedor.ToLower();
        //    string blobNameLower = blobName.ToLower();
        //    string pathLower = Path.ToLower();

        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

        //    // Se crea el blob client
        //    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        //    //Conecta el contenedor a partir del nombre del contenedor que ha recibido
        //    CloudBlobContainer container = blobClient.GetContainerReference(contenedorLower);

        //    //Si no existe, crea el contenedor
        //    container.CreateIfNotExists();

        //    String[] a = pathLower.Split('\\');
        //    String b = a.Last();
        //    WriteLog("Se ha recortado archivolocal mediante split:  " + b);

        //    CloudBlockBlob blockBlob = container.GetBlockBlobReference(b);

        //    // await blockBlob.UploadFromStreamAsync(System.IO.File.OpenRead(pathLower));

        //    try
        //    {
        //        using (var fileStream = System.IO.File.OpenRead(@pathLower))
        //        {
        //            WriteLog("SUBIENDO" + pathLower + " AL CONTAINER " + container.Name);
        //            blockBlob.UploadFromFile(pathLower);
        //        }
        //        WriteLog("SUBIDO" + pathLower + " AL CONTAINER " + container.Name);
        //    }
        //    catch (Exception e)
        //    {
        //        WriteLog("ERROR SUBIENDO" + pathLower + " AL CONTAINER " + container.Name + "\n" + e.ToString());
        //    }

        //    finally
        //    {
        //        System.IO.File.Delete(Path);
        //    }
        //}



        protected virtual void PrintTicket(string Path)
        {
            try
            {
               

                try
                {
                    
                    try
                    {
                       

                    }
                    finally
                    {

                    }
                }
                catch (Exception ex)
                {
                    WriteLog("Error al imprimir: " + Path + " - " + ex.Message + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
                }

            }
            catch (Exception ex)
            {
                WriteLog("Error al imprimir: " + Path + " - " + ex.Message + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
            }
        }

        protected async void CheckLock(string Path)
        {
            string Behaviour = ConfigurationManager.AppSettings["Behaviour"];

            FileInfo file = new FileInfo(Path);
            var isLocked = true;
            while (isLocked == true)
            {
                isLocked = IsFileLocked(file);
                await Task.Delay(100);
            }

            if (isLocked == false)
            {
                switch (Behaviour)
                {
                    case "Client":
                        PrintTicket(Path);
                        break;
                    case "Server":
                        WriteLog("Esta versión de SaaS printing service no soporta el comportamiento 'Server'");
                        break;
                }

            }


        }

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

        public string GetSubstringByString(string a, string b, string c)
        {
            return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
        }
    }
}
