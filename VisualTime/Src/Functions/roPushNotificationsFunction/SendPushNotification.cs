using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Robotics.Azure;
using Robotics.Base.DTOs;
using Robotics.VTBase;
using System;
using System.Diagnostics;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace roPushNotificationsFunction
{
    public class SendPushNotification
    {
        public static MemoryCache memoryCache = MemoryCache.Default;
        private readonly ILogger _logger;
        private static FirebaseApp defaultApp = null;

        public SendPushNotification(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SendPushNotification>();
        }

        [Function("SendPushNotification")]
        public void Run([ServiceBusTrigger("pushnotificationtasks", Connection = "AzureWebJobsServiceBus")] string myQueueItem, ILogger log)
        {
            Robotics.Azure.RoAzureSupport.SetDefaultLogLevel(Program.DefaultLogLevel, Program.DefaultTraceLevel, "roPushNotificationsFunction");

            try
            {
                roNotificationItem oNotification = JsonConvert.DeserializeObject<roNotificationItem>(myQueueItem);

                roAzureConfig firebaseCache = (roAzureConfig)memoryCache["oFirebaseConfiguration"];

                if (firebaseCache == null)
                {
                    var oCompanyConfigurationRepository = new roConfigRepository();
                    firebaseCache = oCompanyConfigurationRepository.GetConfigParameter(roConfigParameter.firebase);
                    memoryCache.Set("oFirebaseConfiguration", firebaseCache, DateTimeOffset.Now.AddDays(1));
                }

                if (firebaseCache != null && SendPushNotification.defaultApp == null)
                {
                    SendPushNotification.defaultApp = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromJson(firebaseCache.value)
                    });
                }

                Task.Run(() => SendPushMessage(oNotification.Destination, oNotification.Subject, oNotification.Body));
            }
            catch (Exception e)
            {
                roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roPushNotificationsFunction::SendPushNotification::Fatal error::Exception::" + e.Message);
            }


        }

        private static async Task<string> SendPushMessage(string token, string title, string body)
        {
            string result;
            roConstants.InitializeFunctionCallEnvironment("SendPushNotification");
            var messaging = FirebaseMessaging.DefaultInstance;

            try
            {
                result = await messaging.SendAsync(new FirebaseAdmin.Messaging.Message()
                {
                    Token = token,
                    Notification = new Notification()
                    {
                        Body = body,
                        Title = title
                    }
                }).ConfigureAwait(false);


            }
            catch (Exception e)
            {
                //Esta notificacíón que se ignora se corresponde a un token dado de baja.
                if (!e.Message.Contains("Requested entity was not found")) roLog.get_GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roPushNotificationsFunction::SendPushNotification::Fatal error::Exception::" + e.Message);
                result = "";
            }

            roTrace.get_GetInstance().AddTraceEvent("PushSent::OK::" + token);
            roTrace.get_GetInstance().TraceMessage(roTrace.TraceType.roInfo, roTrace.TraceResult.Ok, "Push");

            return result;
        }
    }
}