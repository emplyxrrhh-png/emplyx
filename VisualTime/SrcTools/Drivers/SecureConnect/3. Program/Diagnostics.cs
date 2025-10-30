using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VisualtimeSecureConnect.DTOs;

namespace NetProxy
{
    static class Diagnostics
    {
        public static void RunDiagnostics()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{Program.proxyConfiguration.diagnosticsPort}/");

            try
            {
                listener.Start();
                Logger.Log($"Diagnostics site running on http://localhost:{Program.proxyConfiguration.diagnosticsPort}/...");

                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    // TODO: Construir la página de diagnóstico
                    string responseHtml = GetDiagnosticHTML();

                    byte[] buffer = Encoding.UTF8.GetBytes(responseHtml);
                    response.ContentLength64 = buffer.Length;

                    using (var output = response.OutputStream)
                    {
                        output.Write(buffer, 0, buffer.Length);
                    }

                    response.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception: Fatal error processing on diagnostics page request: {ex.Message}\n{ex.InnerException}");
            }
            finally
            {
                listener.Close();
            }
        }

        //Página de diagnóstico
        public static string GetDiagnosticHTML()
        {
            string htmlCode = @"
                                <!DOCTYPE html>
                                <html>
                                <head>
                                    <meta charset=""UTF-8"">
                                    <title>Visualtime Secure Connect Monitor</title>
                                    <style>
                                        body {
                                            font-family: Arial, sans-serif;
                                            background-color: #f4f4f4;
                                            margin: 0;
                                            padding: 0;
                                            text-align: center;
                                        }

                                        h1 {
                                            background-color: #007BFF;
                                            color: #fff;
                                            padding: 20px;
                                        }

                                        .container {
                                            display: flex;
                                            flex-direction: column;
                                            align-items: center;
                                            justify-content: center;
                                            height: 100vh;
                                        }

                                        table {
                                            border-collapse: collapse;
                                            width: 40%;
                                            margin-top: 20px;
                                        }

                                        th, td {
                                            border: 1px solid #ccc;
                                            padding: 10px;
                                            text-align: center;
                                            vertical-align: middle;
                                        }

                                        th {
                                            background-color: #007BFF;
                                            color: #fff;
                                        }

                                        .ok {
                                            color: green; 
			                                font-weight: bold;
                                        }
		
		                                .ko {
                                            color: red; 
			                                font-weight: bold;
                                        }

                                        .bold-text {
                                            font-weight: bold; /* Hacer que el texto sea en negrita */
                                        }
                                    </style>
                                </head>
                                <body>
                                    <h1>Visualtime Secure Connect Monitor</h1>
                                    <div class=""container"">
                                        <table>
                                            <tr>
                                                <th>Estado</th>
                                                <td><span class=""###STATUSCOLOR###"">###STATUS###</span></td>
                                            </tr>
                                            <tr>
                                                <th>Identificador de cliente</th>
                                                <td><span class=""bold-text"">###CUSTOMER###</span></td>
                                            </tr>
                                            <tr>
                                                <th>Comportamiento</th>
                                                <td><span class=""bold-text"">###TUNELSIDE###</span></td>
                                            </tr>
                                            <tr>
                                                <th>DNSs destino</th>
                                                <td><span class=""bold-text"">###FORWARDDNS###</span></td>
                                            </tr>
                                            <tr>
                                                <th>Endpoints</th>
                                                <td class=""bold-text"">###ENDPOINTS###</td>
                                            </tr>
                                            <tr>
                                                <th>Fuente configuración endpoints</th>
                                                <td class=""###CONFSOURCECOLOR###"">###CONFIGSOURCE###</td>
                                            </tr>
                                            <tr>
                                                <th>Consulta configuración endpoints cada</th>
                                                <td class=""###CHECKCONFIGEVERYCOLOR###"">###CHECKCONFIGEVERY###</td>
                                            </tr>
                                        </table>
                                    </div>
                                </body>
                                </html>
                    ";

            htmlCode = htmlCode.Replace("###STATUS###", Program.diagnosticsInfo.status.ToString());
            htmlCode = htmlCode.Replace("###STATUSCOLOR###", (Program.diagnosticsInfo.status.ToString().ToLower() == "running") ? "ok" : "ko");
            htmlCode = htmlCode.Replace("###CUSTOMER###", Program.diagnosticsInfo.customerId);
            htmlCode = htmlCode.Replace("###TUNELSIDE###", (Program.diagnosticsInfo.role.ToLower() == "terminal") ? "cliente" : "servidor");
            htmlCode = htmlCode.Replace("###FORWARDDNS###", Program.diagnosticsInfo.forwardIp);
            htmlCode = htmlCode.Replace("###ENDPOINTS###", EndPointsFormated(Program.endPointsConfiguration));
            htmlCode = htmlCode.Replace("###CONFIGSOURCE###", Program.diagnosticsInfo.endpointsConfigSource);
            htmlCode = htmlCode.Replace("###CONFSOURCECOLOR###", (Program.diagnosticsInfo.endpointsConfigSource == "api") ? "ok" : "ko");
            int minutes = Program.diagnosticsInfo.checkEndpointsEvery;
            string checkEvery = ConvertMinutesToHoursAndMinutes(minutes);           
            htmlCode = htmlCode.Replace("###CHECKCONFIGEVERY###", checkEvery);
            htmlCode = htmlCode.Replace("###CHECKCONFIGEVERYCOLOR###", (checkEvery == "nunca" && Program.diagnosticsInfo.role.ToLower() == "terminal") ? "ko" : "ok");


            return htmlCode;

        }

        private static string EndPointsFormated(EndPointsConfiguration config)
        {
            StringBuilder result = new StringBuilder();

            try
            {
                foreach (var kvp in config.endPoints)
                {
                    VisualtimeSecureConnect.DTOs.EndPoint endPoint = kvp.Value;

                    result.Append($"{kvp.Key}:{endPoint.localPort} -> {endPoint.forwardIp}:{endPoint.forwardPort}<br>");
                }
            }
            catch (Exception) { }

            return result.ToString();
        }

        public static string ConvertMinutesToHoursAndMinutes(int minutes)
        {
            string result = "nunca";

            if (minutes == 0) return result;

            int hours = minutes / 60;
            int remainingMinutes = minutes % 60;

            result = $"{hours} horas y {remainingMinutes} minutos";

            if (remainingMinutes == 0) {
                result = $"{hours} horas";
            } else if (hours == 0) {
                result = $"{remainingMinutes} minutos";
            } 

            return result;
        }
    }

}

