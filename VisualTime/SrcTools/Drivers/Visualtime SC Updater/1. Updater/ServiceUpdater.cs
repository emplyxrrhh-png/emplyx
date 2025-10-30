using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCServiceUpdater;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;

namespace SCUpdater
{
    internal class ServiceUpdater
    {

        private static ServiceUpdater? _instance;
        private static readonly object Lock = new object();

        private ServiceUpdater() { }

        public static ServiceUpdater Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ServiceUpdater();
                        }
                    }
                }
                return _instance;
            }
        }
        public bool UpdateService(ProxyConfiguration configuration)
        {
            var updateInfo = FetchUpdateInfo(configuration);

            if (updateInfo == null)
            {
                Logger.LogEvent("SCUpdater: Error getting API update info");
            }
            else 
            {
                if (!updateInfo.hasVersion)
                {
                    Logger.LogEvent("SCUpdater: I already have max version.");
                }
                else
                {
                    if (updateInfo?.updater?.content == null)
                    {
                        Logger.LogEvent("SCUpdater: I do have to update but filecontent doesn't exist. I don't update version.");
                    }
                    else
                    {
                        //Actualizo
                        if (Helper.StopService())
                        {
                            DirectoryInfo infoDirectorioActual = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                            ExtractZipFromByteArray(updateInfo.updater.content, infoDirectorioActual.FullName);

                            if (Helper.StartService())
                            {
                                configuration.version = updateInfo.updater.fileVersion;
                                if (Helper.SaveProxyConfiguration(configuration))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }         
            return false;
        }

        public void ExtractZipFromByteArray(byte[] zipBytes, string destinationFolder)
        {
            using (MemoryStream zipStream = new MemoryStream(zipBytes))
            {
                using (ZipArchive archive = new ZipArchive(zipStream))
                {

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string? entryPath = Path.Combine(destinationFolder, entry.FullName);
                        if (!string.IsNullOrEmpty(entryPath))
                        {
                            string directoryName = Path.GetDirectoryName(entryPath) ?? string.Empty;
                            Directory.CreateDirectory(directoryName);
                        }

                        if (entry.Length > 0)
                        {
                            using (Stream entryStream = entry.Open())
                            {
                                using (FileStream fileStream = File.Open(entryPath, FileMode.Create))
                                {
                                    entryStream.CopyTo(fileStream);
                                }
                            }
                        }
                    }
                }
            }
        }

        private ApiResponse? FetchUpdateInfo(ProxyConfiguration? configuration)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string? apiUrl = configuration?.updateApiUrl;
                    string? clientId = configuration?.customerId;
                    string? token = configuration?.apiToken;
                    string? version = configuration?.version;
                    string? updateInfoUrl = $"{apiUrl}/{clientId}/{token}/{version}";
                    HttpResponseMessage response = client.GetAsync(updateInfoUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = response.Content.ReadAsStringAsync().Result;
                        ApiResponse? updateInfo = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
                        return updateInfo;
                    }
                    else 
                    { 
                        Logger.LogEvent(($"Error: API request failed with status code {response.StatusCode}"));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogEvent("Error fetching update information: " + ex.ToString());
            }

            return null;
        }

    }
}
