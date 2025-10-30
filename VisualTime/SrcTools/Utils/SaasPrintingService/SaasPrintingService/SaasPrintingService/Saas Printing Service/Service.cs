using System;
using System.Drawing;
using System.IO;
using System.Drawing.Printing;
using System.Linq;
using System.ServiceProcess;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using Saas_Printing_Service;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace SaasPrintingService
{
    public partial class Service : ServiceBase
    {
        private StreamReader streamToPrint;
        private Font printFont;

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)

        {
            string Behaviour = ConfigurationManager.AppSettings["Behaviour"];

            switch (Behaviour)
            {
                case "Client":
                    ClientBehaviour();
                    break;

                case "Server":
                    ServerBehaviour();
                    break;
            }
        }

        protected void ClientBehaviour()
        {
            Utils.WriteLog("Inicio del Servicio en Cliente: ");

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
                Utils.WriteLog("Servicio detenido: " + ex.Message);
            }

            string Blob = ConfigurationManager.AppSettings["Blob"];
            string Container = ConfigurationManager.AppSettings["Container"];

            bool isMTKey = ConfigurationManager.AppSettings.AllKeys.ToList().Find(x => x == "IsMT") != null ? bool.Parse(ConfigurationManager.AppSettings["IsMT"].ToLower()) : false;

            CloudStorageAccount storageAccount = null;
            CloudBlobContainer container = null;
            String folder = String.Empty;
            if (!isMTKey)
            {
                Utils.WriteLog("Inicio entorno SaaS/OnPrem: ");
                storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                Utils.WriteLog("... Conectando al contender: " + Container.ToLower());
                container = blobClient.GetContainerReference(Container.ToLower());
            }
            else
            {
                string storageFolder = ConfigurationManager.AppSettings["StorageFolder"];
                if (!string.IsNullOrEmpty(storageFolder))
                {
                    folder = $"{storageFolder.Trim().ToLower()}";
                }
                else
                {
                    folder = "diner";
                }

                Utils.WriteLog("Inicio entorno MT: ");
                var storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
                if (!storageConnectionString.StartsWith("?"))
                {
                    storageConnectionString  = "?" + storageConnectionString;
                }
                StorageCredentials accountSAS = new StorageCredentials(ConfigurationManager.AppSettings["StorageConnectionString"]);

                storageAccount = new CloudStorageAccount(accountSAS, ConfigurationManager.AppSettings["StorageKey"], "", true);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                container = blobClient.GetContainerReference(Container.ToLower());
            }

            if (container != null)
            {
                Utils.WriteLog("... Container de cliente verificado y correcto. ");

                var BlobSyncro = new BlobSync(container, LocalPath, folder, TimeSpan.FromSeconds(1));
                BlobSyncro.Start();
            }
            else
            {
                Utils.WriteLog("... No se ha podido obtener la referencia del contenedor. ");
            }
        }

        protected void ServerBehaviour()
        {
            Utils.WriteLog("Inicio del Servicio en Servidor: ");
            string Path = ConfigurationManager.AppSettings["ServerPath"];

            System.IO.DirectoryInfo di = new DirectoryInfo(Path);

            foreach (FileInfo file in di.GetFiles())
            {
                Utils.WriteLog(file.Extension);

                if ((file.CreationTimeUtc < (DateTime.UtcNow - new TimeSpan(0, 5, 0))) || (file.Extension != ".txt"))
                {
                    file.Delete();
                    Utils.WriteLog("Se ha eliminado por caducidad o formato incorrecto el archivo: " + file.Name + " " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
                }
                else
                {
                    CheckLock(file.FullName);
                    Utils.WriteLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff tt") + " Archivo atrasado detectado en el servidor= " + file.Name +  " ");
                }
            }

            try
            {
                FileSystemWatcher fw = new FileSystemWatcher();
                fw.Path = @Path;
                fw.IncludeSubdirectories = true;
                fw.Filter = "*.*";
                fw.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                    | NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.FileName
                                    | NotifyFilters.Security | NotifyFilters.Size;

                fw.Created += fw_CreatedOnServer;
                fw.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Servicio detenido: " + ex.Message + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
            }
        }

        private void cw_DetectedOnClient(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.Contains("FsLog"))
            {
                CheckLock(e.FullPath);
                Utils.WriteLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff tt") + " Archivo detectado desde el cliente= " + e.ChangeType + " Ruta = " + e.FullPath + " ");
            }
        }

        private void fw_CreatedOnServer(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.Contains("FsLog"))
            {
                CheckLock(e.FullPath);
                Utils.WriteLog(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff tt") + " Archivo creado en el servidor= " + e.ChangeType + " Ruta = " + e.FullPath + " ");
            }
        }

        protected void UploadTicket(string contenedor, string blobName, string Path)
        {
            string contenedorLower = contenedor.ToLower();
            string blobNameLower = blobName.ToLower();
            string pathLower = Path.ToLower();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Se crea el blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Conecta el contenedor a partir del nombre del contenedor que ha recibido
            CloudBlobContainer container = blobClient.GetContainerReference(contenedorLower);

            //Si no existe, crea el contenedor
            container.CreateIfNotExists();

            String[] a = pathLower.Split('\\');
            String b = a.Last();
            Utils.WriteLog("Se ha recortado archivo local mediante split:  " + b);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(b);

            // await blockBlob.UploadFromStreamAsync(System.IO.File.OpenRead(pathLower));

            try
            {
                using (var fileStream = System.IO.File.OpenRead(@pathLower))
                {
                    Utils.WriteLog("SUBIENDO" + pathLower + " AL CONTAINER " + container.Name);
                    blockBlob.UploadFromFile(pathLower);
                }
                Utils.WriteLog("SUBIDO" + pathLower + " AL CONTAINER " + container.Name);
            }
            catch (Exception e)
            {
                Utils.WriteLog("ERROR SUBIENDO" + pathLower + " AL CONTAINER " + container.Name + "\n" + e.ToString());
            }
            finally
            {
                Utils.WriteLog("Fichero borrado");
                System.IO.File.Delete(Path);
            }
        }

        protected override void OnStop()
        {
            Utils.WriteLog("Servicio detenido: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;

            // Calcula la cantidad de líneas por página
            linesPerPage = ev.MarginBounds.Height / (float)printFont.GetHeight(ev.Graphics);
            // Imprime cada línea del fichero
            while (count < linesPerPage)
            {
                line = streamToPrint.ReadLine();
                if (line == null)
                    break;
                Font bigFont = new Font(SystemFonts.DefaultFont.FontFamily, float.Parse(GetSubstringByString("$", "}", line)), FontStyle.Regular);

                // Condicional para incorporar tamaños de letras custom por lineas, entemos que siempre seran 3 lineas, pero esta preparado para tener más o menos lineas.
                if (ConfigurationManager.AppSettings.AllKeys.ToList().Find(x => x == $"PrinterLine{count}Size") != null ? true : false)
                {
                    var size = "12";
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[$"PrinterLine{count}Size"]))
                    {
                        size = ConfigurationManager.AppSettings[$"PrinterLine{count}Size"];
                    }
                    bigFont = new Font(SystemFonts.DefaultFont.FontFamily, float.Parse(size), FontStyle.Regular);
                }

                var linea = line.Substring(line.LastIndexOf('}') + 1);

                yPos = topMargin + count * printFont.GetHeight(ev.Graphics);
                ev.Graphics.DrawString(linea, bigFont, Brushes.Black, leftMargin, yPos, new StringFormat());
                count += 1;
            }

            // Si existen más lineas, imprime en otra página
            if ((line != null))
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }

        protected virtual void PrintTicket(string Path)
        {
            try
            {
                // Se ha de imprimir el ticket
                PrintDocument pd = new PrintDocument();

                try
                {
                    Utils.WriteLog("Imprimiendo ticket: " + Path + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
                    streamToPrint = new StreamReader(@Path);
                    try
                    {
                        printFont = new Font("Arial", 10);
                        Margins margins = new Margins(Int32.Parse(ConfigurationManager.AppSettings["PrinterMarginLeft"]), Int32.Parse(ConfigurationManager.AppSettings["PrinterMarginLeft"]),
                         Int32.Parse(ConfigurationManager.AppSettings["PrinterMarginTop"]), Int32.Parse(ConfigurationManager.AppSettings["PrinterMarginTop"]));
                        pd.DefaultPageSettings.Margins = margins;
                        pd.PrintPage += this.pd_PrintPage;
                        pd.PrinterSettings.PrinterName = ConfigurationManager.AppSettings["PrinterName"];
                        pd.Print();
                        Utils.WriteLog("Ticket impreso: " + Path + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLog("Error al imprimir1: " + Path + " - " + ex.ToString() + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
                    }
                    finally
                    {
                        streamToPrint.Close();
                        System.IO.File.Delete(Path);
                        Utils.WriteLog("Fichero temporal eliminado: " + Path + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
                    }
                }
                catch (Exception ex)
                {
                    Utils.WriteLog("Error al imprimir2: " + Path + " - " + ex.Message + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
                }
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Error al imprimir3: " + Path + " - " + ex.Message + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
            }
        }

        protected virtual bool PrintHtmlPages(
    string printer,
    List<string> urls)
        {
            try
            {
                var info = new ProcessStartInfo();
                info.Arguments = $"-l 0.1 -t 0 -r 0.1 -b 0.1 -p \"{printer}\" \"{string.Join("\" \"", urls)}\"";
                var pathToExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                info.FileName = Path.Combine(pathToExe, "PrintHTML\\PrintHtml.exe");
                using (var p = Process.Start(info))
                {
                    while (!p.HasExited)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        System.Threading.Thread.Sleep(10);
                    }

                    return p.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        protected async void CheckLock(string Path)
        {
            string Container = ConfigurationManager.AppSettings["Container"];
            string Behaviour = ConfigurationManager.AppSettings["Behaviour"];
            string Blob = ConfigurationManager.AppSettings["Blob"];
            FileInfo file = new FileInfo(Path);
            var isLocked = true;
            while (isLocked == true)
            {
                isLocked = IsFileLocked(file);
                await Task.Delay(100);
            }

            if (isLocked == false)
            {
                if (!(Behaviour == "Client"))
                {
                    if (Behaviour == "Server")
                    {
                        UploadTicket(Container, Blob, Path);
                    }
                }
                else
                {
                    var fileContent = System.IO.File.ReadAllText(Path);
                    if (!Utils.IsHtmlContent(fileContent))
                    {
                        PrintTicket(Path);
                    }
                    else
                    {
                        ModifyHtmlToAddUtf8(Path, fileContent);
                        PrintHtmlPages(ConfigurationManager.AppSettings["PrinterName"], new List<string> { Path });
                        System.IO.File.Delete(Path);
                    }
                }
            }
        }

        private static void ModifyHtmlToAddUtf8(string path, string fileContent)
        {
           var htmlContent = EnsureUtf8Encoding(fileContent);

            File.WriteAllText(path, htmlContent);
        }

        private static string EnsureUtf8Encoding(string html)
        {
            //Por si alguna vez viene asi por lo que sea desde el servidor
            if (html.Contains("<meta charset=\"UTF-8\">"))
            {
                return html;
            }

            int pIndex = html.IndexOf("<p>");
            if (pIndex != -1)
            {
                // Sumamos 3 para insertar despues del <p>
                html = html.Insert(pIndex + 3, "<meta charset=\"UTF-8\">");
            }

            return html;
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