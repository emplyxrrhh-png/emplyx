using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Robotics.Base.DTOs;
using Robotics.VTBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace roSCFunctions
{
    public class RunSyncFiles
    {
        private readonly ILogger _logger;

        public RunSyncFiles(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RunSyncFiles>();
        }

        [Function("RunSyncFilesTask")]
        public void RunSyncFilesTask([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            int splitGroupSize = roTypes.Any2Integer(Robotics.VTBase.roConstants.GetConfigurationParameter("SplitGroupSize"));
            if (splitGroupSize <= 0) splitGroupSize = 100;

            Robotics.Azure.RoAzureSupport.SetDefaultLogLevel(Program.DefaultLogLevel, Program.DefaultTraceLevel, "roSCFunctions");
            roConstants.InitializeFunctionCallEnvironment("SyncStorageConnector");

            if (Robotics.DataLayer.roCacheManager.GetInstance.CheckUpdateCache()) Robotics.DataLayer.roCacheManager.GetInstance.ClearCompaniesCache();

            roCompanyConfiguration[] cachedCompanies = Robotics.DataLayer.roCacheManager.GetInstance.GetCompanies();

            RunSyncFiles.CheckPNlinkEnabled(cachedCompanies, splitGroupSize);

            roTrace.get_GetInstance().TraceMessage(roTrace.TraceType.roInfo, roTrace.TraceResult.Ok, "Time trigger");
        }

        public static void CheckPNlinkEnabled(roCompanyConfiguration[] companies, int splitGroupSize)
        {
            try
            {
                DefaultAzureCredential cred = new DefaultAzureCredential();
                string[] scopes = { "https://storage.azure.com/.default" };
                var token = cred.GetToken(new TokenRequestContext(scopes));

                bool debugMode = roTypes.Any2Boolean(Robotics.VTBase.roConstants.GetConfigurationParameter("DebugMode"));
                if (debugMode) roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, $"Starting pnlink company check with auth{token.Token}");

                List<int> iSplitItems = new List<int>();

                int iPos = 0;
                iSplitItems.Add(iPos);
                iPos += splitGroupSize;

                while (iPos < companies.Length)
                {
                    iSplitItems.Add(iPos);
                    iPos += splitGroupSize;
                }

                roTrace.get_GetInstance().AddTraceEvent($"Checking for {companies.Length} companies pnet enabled");

                Dictionary<String, String> o11yDic = roTelemetryInfo.GetInstance.GetO11yInfo();
                Dictionary<String, String> traceDic  = roTelemetryInfo.GetInstance.GetTraceInfo();
                roThreadData oThreadData = roConstants.BackupThreadData();

                System.Threading.Tasks.Parallel.ForEach(iSplitItems, number =>
                {
                    roCompanyConfiguration[] tmpCompanies = companies.Skip(number).Take(splitGroupSize).ToArray();
                    RunSyncFiles.CheckPNlinkEnabledParallel(o11yDic, traceDic, oThreadData, tmpCompanies, token.Token, Program.DefaultLogLevel, Program.DefaultTraceLevel);
                });
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, $"CheckPNlinkEnabled::Unhandled exception::{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        public static int CheckPNlinkEnabledParallel(Dictionary<String, String> o11yDic, Dictionary<String, String> traceDic, roThreadData sourceThreadData, roCompanyConfiguration[] companies, string token, int defaultLogLevel, int defaultTraceLevel)
        {
            try
            {
                foreach (roCompanyConfiguration oCompany in companies)
                {
                    if (string.IsNullOrEmpty(oCompany.dbconnectionstring))
                        continue;

                    if (Robotics.DataLayer.AccessHelper.SetThreadCompanyInformation(sourceThreadData, o11yDic, traceDic, oCompany))
                        RunSyncFiles.CheckPNlinkEnabledForCompany(oCompany, token);

                    Robotics.DataLayer.AccessHelper.ClearThreadCompanyInformation();
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, $"CheckPNlinkEnabledParallel::Unhandled exception::{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            return 0;
        }

        public static int CheckPNlinkEnabledForCompany(roCompanyConfiguration company, string token)
        {
            try
            {
                string scURL = Environment.GetEnvironmentVariable("StorageConnector.URL");

                string pnLinkName = (string)Robotics.DataLayer.roCacheManager.GetInstance.GetAdvParametersCache(company.Id).AdvancedParameters["pnlink.companyname"];
                if (pnLinkName != null && pnLinkName != string.Empty)
                {
                    List<string> sourceFiles = Robotics.Azure.RoAzureSupport.ListFiles("*", "encrypted", roLiveQueueTypes.pnlink, "", true, pnLinkName);

                    foreach (string sourceFile in sourceFiles)
                    {
                        roTrace.get_GetInstance().AddTraceEvent($"Sync file '{sourceFile}' for client {company.Id}");

                        try
                        {
                            string destFile = sourceFile.Replace(".encrypted", "");

                            System.IO.MemoryStream inputFile = null;
                            try
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                                    HttpResponseMessage response = Task.Run(async () => await client.GetAsync($"{scURL}/api/EncryptedStorage/{pnLinkName}/{destFile}?createContainer=true")).GetAwaiter().GetResult();
                                    if (response.IsSuccessStatusCode)
                                    {
                                        // Obtiene el contenido del archivo Excel como bytes
                                        byte[] excelBytes = Task.Run(async () => await response.Content.ReadAsByteArrayAsync()).GetAwaiter().GetResult();

                                        inputFile = new System.IO.MemoryStream(excelBytes);
                                    }
                                    else
                                    {
                                        roLog.get_GetInstance().logMessage(roLog.EventType.roInfo, $"CheckPNlinkEnabledForCompany::File {sourceFile} could not be decrypted sc response code {response.StatusCode}");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                roLog.get_GetInstance().logMessage(roLog.EventType.roError, $"CheckPNlinkEnabledForCompany::Unhandled exception decrypting file {sourceFile}::{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                            }

                            try
                            {
                                if (inputFile != null)
                                    Robotics.Azure.RoAzureSupport.UploadStream2BlobInCompanyContainer(inputFile, ("import/" + destFile), roLiveQueueTypes.datalink, company.Id);
                            }
                            catch (Exception ex)
                            {
                                roLog.get_GetInstance().logMessage(roLog.EventType.roError, $"CheckPNlinkEnabledForCompany::Unhandled exception uploading vt file {sourceFile}::{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                            }

                            try
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                                    HttpResponseMessage response = Task.Run(async () => await client.DeleteAsync($"{scURL}/api/EncryptedStorage/{pnLinkName}/{destFile}?createContainer=true")).GetAwaiter().GetResult(); ;
                                    if (!response.IsSuccessStatusCode)
                                    {
                                        roLog.get_GetInstance().logMessage(roLog.EventType.roInfo, $"CheckPNlinkEnabledForCompany::File {sourceFile} could not be removed from sc response code {response.StatusCode}");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                roLog.get_GetInstance().logMessage(roLog.EventType.roError, $"CheckPNlinkEnabledForCompany::Unhandled exception removing file {sourceFile}::{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                            }

                            roLog.get_GetInstance().logMessage(roLog.EventType.roDebug, $"CheckPNlinkEnabledForCompany::File {sourceFile} successfully added to visualtime");
                        }
                        catch (Exception ex)
                        {
                            roLog.get_GetInstance().logMessage(roLog.EventType.roError, $"CheckPNlinkEnabledForCompany::File {sourceFile} could not be imported by {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                roLog.get_GetInstance().logMessage(roLog.EventType.roError, $"CheckPNlinkEnabledForCompany::Unhandled exception::{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            return 0;
        }
    }
}