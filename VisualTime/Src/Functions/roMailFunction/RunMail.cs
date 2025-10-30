using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Robotics.Azure;
using Robotics.Base.DTOs;
using Robotics.VTBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using System.Runtime.Caching;

namespace roMailFunction
{
    public class RunMail
    {
        public static MemoryCache memoryCache = MemoryCache.Default;
        private readonly ILogger _logger;

        public RunMail(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RunMail>();
        }

        [Function("RunMail")]
        public void Run([ServiceBusTrigger("emailtasks", Connection = "AzureWebJobsServiceBus")] string myQueueItem, ILogger log)
        {
            Robotics.Azure.RoAzureSupport.SetDefaultLogLevel(Program.DefaultLogLevel, Program.DefaultTraceLevel, "roMailFunction");
            roConstants.InitializeFunctionCallEnvironment("RunMail");
            roTrace.TraceResult result = roTrace.TraceResult.Ok;
            try
            {
                string body = myQueueItem;

                var strCompany = body.Split('@')[0];
                var strTaskId = body.Split('@')[1];
                var strMessage = body.Replace((strCompany + "@" + strTaskId + "@"), "");

                strMessage = Robotics.VTBase.CompressionHelper.DecompressString(strMessage);

                roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roDebug, "roMailFunction::SendingNotification::Company::" + strCompany + "::Task::" + strTaskId);

                if (strTaskId == "-999") Robotics.DataLayer.roCacheManager.GetInstance.ClearCompaniesCache();

                roAzureConfig emailCache = (roAzureConfig)memoryCache["oEmailConfiguration"];

                bool bResetEmailClient = false;

                if (emailCache == null || strTaskId == "-999")
                {
                    var oCompanyConfigurationRepository = new roConfigRepository();
                    emailCache = oCompanyConfigurationRepository.GetConfigParameter(roConfigParameter.email);
                    memoryCache.Set("oEmailConfiguration", emailCache, DateTimeOffset.Now.AddDays(1));

                    if (strTaskId == "-999") bResetEmailClient = true;

                    roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roDebug, "roMailFunction::RunMail::LoadEmailConfiguration");
                    roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roDebug, "roMailFunction::EmailConfig::Value::" + emailCache.value);
                }

                if (strTaskId != "-999")
                {
                    roNotificationItem oSendItem = null;
                    try
                    {
                        oSendItem = (roNotificationItem)Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(strMessage, typeof(roNotificationItem));
                    }
                    catch (Exception e)
                    {
                        roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roMailFunction::SendItem::JSON Incorrect", e);
                        oSendItem = null;
                        result = roTrace.TraceResult.NotConfigured;
                    }

                    if (oSendItem != null)
                    {
                        switch (oSendItem.Type)
                        {
                            case NotificationItemType.sms:
                                try
                                {
                                    roSendSMS.roSendMessage.SendMessage.SendMessage(oSendItem.Content);
                                }
                                catch (Exception e)
                                {
                                    result = roTrace.TraceResult.Error;
                                    roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roMailFunction::SendSms::Error::", e);
                                }
                                break;

                            case NotificationItemType.email:
                                try
                                {
                                    roMailConfig oConf = (roMailConfig)Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(emailCache.value, typeof(roMailConfig));
                                    SmtpClient oSmtpServer = Robotics.Mail.SendMail.GetInstance(oConf, bResetEmailClient);

                                    string sendMailResult = Robotics.Mail.SendMail.SendMail(oSendItem.Destination, oSendItem.Subject, oSendItem.Body, "", null, oSmtpServer, oConf.MailAccount, strCompany);
                                    if (sendMailResult != "OK")
                                    {
                                        result = roTrace.TraceResult.Error;
                                        if (strTaskId != "-1") SetNotificationAsNotSended(strCompany, roTypes.Any2Integer(strTaskId));
                                        Robotics.Mail.SendMail.GetInstance(oConf, true);
                                    }
                                }
                                catch (Exception e)
                                {
                                    result = roTrace.TraceResult.Error;
                                    roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roMailFunction::SendEmail::Error::", e);
                                }
                                break;
                        }
                    }
                    else
                    {
                        roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roMailFunction::SendItem::Unrecognized message::" + strMessage);
                    }
                }
            }
            catch (Exception e)
            {
                result = roTrace.TraceResult.Error;
                roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roMailFunction::RunMail::Fatal error::Exception::" + e.Message);
            }

            roTrace.get_GetInstance().TraceMessage(roTrace.TraceType.roInfo, result, "Mail");
        }

        private static void SetNotificationAsNotSended(string strCompany, int strTaskId)
        {
            try
            {
                Dictionary<String, String> o11yDic = roTelemetryInfo.GetInstance.GetO11yInfo();
                Dictionary<String, String> traceDic = roTelemetryInfo.GetInstance.GetTraceInfo();
                roThreadData oThreadData = roConstants.BackupThreadData();
                roCompanyConfiguration oCompanyConfiguration = Robotics.DataLayer.roCacheManager.GetInstance.GetCompany(strCompany);

                if (oCompanyConfiguration != null && Robotics.DataLayer.AccessHelper.SetThreadCompanyInformation(oThreadData, o11yDic, traceDic, oCompanyConfiguration))
                {
                    //Solo reseteamos la notificación para que se vuelva a enviar si había un único destino. Si existen varios no hacemos nada.
                    var oSQLNotification = "@UPDATE# sysroNotificationTasks SET Executed = 0, InProgress = 0 WHERE DestinationsNotified = 1 AND ID=" + strTaskId;
                    Robotics.DataLayer.AccessHelper.ExecuteSql(oSQLNotification);
                }

                Robotics.DataLayer.AccessHelper.ClearThreadCompanyInformation();

                roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roDebug, "roMailFunction::roMailFunction::Notification " + strTaskId + "::Set as not sended");
            }
            catch (Exception)
            {
                roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roMailFunction::roMailFunction::Error setting notification as not sended::" + strTaskId);
            }
        }
    }
}