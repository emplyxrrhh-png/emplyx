using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Threading;
using VisualtimeSecureConnect.Interface;
using VisualtimeSecureConnect.DTOs;
using System.IO;
using System.Net.Http;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Intrinsics.Arm;
using System.Data;

namespace NetProxy
{
    class HttpsProxy : IHttpsProxy
    {
        public async Task Start(string remoteServerDns, ushort localPort)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, localPort);
            listener.Start();

            Logger.Log($"TCP listener started on port {localPort} -> packets will be redirected to {remoteServerDns}");
            while (true)
            {
                try
                {
                    TcpClient terminalListener = await listener.AcceptTcpClientAsync();
                    int intLng = await HandleClientAsync(terminalListener, remoteServerDns);
                    if (intLng == 0)
                    {
                        // No se leyeron más datos en la conexión, terminar el bucle
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Exception: Faltal error starting or listening on port {localPort} to send to {remoteServerDns}: {ex.Message}. \nKeep listening ...");
                }
            }
        }

        private async Task<int> HandleClientAsync(TcpClient terminalListener, string remoteServerDns)
        {
            using (terminalListener)
            using (NetworkStream terminalStream = terminalListener.GetStream())
            {
                int bytesRead = 0;
                try
                {
                    byte[] bInputSocket = new byte[819200];
                    bytesRead = await terminalStream.ReadAsync(bInputSocket, 0, bInputSocket.Length);

                    if (bytesRead > 0)
                    {
                        int nullByteIndex = Array.FindIndex(bInputSocket, 0, bytesRead, b => b == 0);

                        string strReceive;
                        if (nullByteIndex >= 0)
                        {
                            // Sólo se procesa la parte de la cadena que no contiene bytes nulos
                            byte[] cleanedData = new byte[nullByteIndex];
                            Array.Copy(bInputSocket, cleanedData, nullByteIndex);
                            strReceive = Encoding.UTF8.GetString(cleanedData);
                        }
                        else
                        {
                            strReceive = Encoding.UTF8.GetString(bInputSocket, 0, bytesRead);
                        }

                        string method = string.Empty, uri = string.Empty, body = string.Empty;

                        string[] parts = strReceive.Split(' ');
                        if (parts.Length >1)
                        {
                            method = parts[0];
                            uri = parts[1];
                        }

                        parts = strReceive.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);

                        if (parts.Length > 1)
                        body = parts[1];

                        if (method.Length > 1)
                        {
                            string apiUrl = remoteServerDns + uri;

#if DEBUG
                            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
#endif

                            using (HttpClient client = new HttpClient())
                            {
                                try
                                {
                                    HttpResponseMessage response = null;
                                    switch (method.Trim())
                                    {
                                        case "GET":
                                            HttpRequestMessage getrequest = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                                            getrequest.Headers.Add("VTSecureConnect", "1");
                                            response = await client.SendAsync(getrequest);
                                            break;
                                        case "POST":
                                            HttpRequestMessage postrequest = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                                            var httpcontent = new StringContent(body, Encoding.UTF8, "text/plain");
                                            postrequest.Content = httpcontent;
                                            postrequest.Headers.Add("VTSecureConnect", "1");
                                            response = await client.SendAsync(postrequest);
                                            break;
                                        default:
                                            Logger.Log("Push message with unknown http verb. No GET nor POST. Ignoring");
                                            break;
                                    }

                                    if (response != null)
                                    {
                                        string responseBody = await response.Content.ReadAsStringAsync();

                                        // Envía la respuesta al terminal
                                        byte[] responseBytes = Encoding.UTF8.GetBytes(responseBody);
                                        terminalStream.Write(responseBytes, 0, responseBytes.Length);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log($"Exception: Fatal error processing PUSH message ({strReceive}): {ex.Message}\n{ex.InnerException}");
                                }
                                ServicePointManager.ServerCertificateValidationCallback = null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Exception: Fatal error processing PUSH message: {ex.Message}\n{ex.InnerException}");
                }

                terminalListener.Close();
                return bytesRead;
            }
        }
    }
}
