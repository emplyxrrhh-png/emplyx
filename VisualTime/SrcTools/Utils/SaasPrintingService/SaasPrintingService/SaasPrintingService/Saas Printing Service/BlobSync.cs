using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SaasPrintingService
{
    public class BlobSync
    {
        private CloudBlobContainer container;
        private string localPath;
        public string LocalPath { get { return localPath; } }
        private TimeSpan interval;
        private Thread syncingThread;
        private Dictionary<string, string> localBlobs = new Dictionary<string, string>();

        private string remotefoler;

        public void WriteLog(string log)
        {
            try
            {
                File.AppendAllLines(ConfigurationManager.AppSettings["LogPath"] + DateTime.Today.ToString("yyyy_MM_dd") + ".log", new string[] { log });
            }
            catch { }
        }

        public BlobSync(CloudBlobContainer container, string localPath, string remotefoler, TimeSpan interval)
        {
            this.container = container;
            this.localPath = localPath;
            this.interval = interval;
            this.remotefoler = remotefoler == "" ? "" : remotefoler + "/";
        }
        public BlobSync(CloudBlobContainer container, string localPath) : this(container, localPath, "", TimeSpan.FromSeconds(5)) { }

        private string GetLocalPath(string uri)
        {
            return Path.Combine(localPath, uri.Substring(container.Uri.AbsoluteUri.Length + 1).Replace('/', '\\'));
        }

        public void SyncAll()
        {
            try
            {
                var cloudBlobs = container.ListBlobs(this.remotefoler, false, BlobListingDetails.Metadata
                ).OfType<CloudBlob>();

                var cloudBlobNames = new HashSet<string>(cloudBlobs.Select(b => b.Uri.ToString()));
      
                foreach (var blob in cloudBlobs)
                {

                    if (!localBlobs.ContainsKey(blob.Uri.ToString()) ||
                        blob.Properties.ETag != localBlobs[blob.Uri.ToString()])
                    {

                        var path = GetLocalPath(blob.Uri.ToString());
                        var args = new UpdatingFileEventArgs(blob, path);

                        Directory.CreateDirectory(Path.GetDirectoryName(path));

                        string archivo = @ConfigurationManager.AppSettings["ClientPath"] + blob.Name;
                        archivo = archivo.Replace("/", @"\");
                        string carpeta = Path.GetDirectoryName(archivo);
                        using (var fileStream = System.IO.File.OpenWrite(archivo))
                        {
                            blob.DownloadToStream(fileStream);

                        }

                        localBlobs[blob.Uri.ToString()] = blob.Properties.ETag;
                        container.GetBlobReference(blob.Name).DeleteIfExists();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }

        }

        public void SyncForever()
        {
            while (true)
            {
                SyncAll();
                Thread.Sleep(interval);
            }
        }

        public void Start()
        {
            syncingThread = new Thread(new ThreadStart(SyncForever));
            syncingThread.Start();
        }

        public void Stop()
        {
            syncingThread.Abort();
        }

        public class UpdatingFileEventArgs
        {
            public CloudBlob Blob;
            public string LocalPath;
            public UpdatingFileEventArgs(CloudBlob blob, string localPath)
            {
                Blob = blob;
                LocalPath = localPath;
            }
        }

    }
}
