using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using VisualtimeSecureConnect.Interface;
using VisualtimeSecureConnect.DTOs;
using System.IO;

namespace NetProxy
{
    class TcpProxy: IProxy
    {
        public async Task Start(string remoteServerIp, ushort remoteServerPort, ushort localPort,string localIp, ushort latency, ProxyRole role)
        {
            IPAddress localIpAddress = string.IsNullOrEmpty(localIp) ? IPAddress.IPv6Any : IPAddress.Parse(localIp);
            var terminalListener = new TcpListener(new IPEndPoint(localIpAddress, localPort));
            terminalListener.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            terminalListener.Start();

            Logger.Log($"TCP listener started on port {localPort} -> packets will be redirected to {remoteServerIp}:{remoteServerPort} with latency = {latency} ms");
            while (true)
            {
                try
                {
                    var terminalClient = await terminalListener.AcceptTcpClientAsync();
                    Logger.Log($"Connection received over port {localPort} ...");
                    terminalClient.NoDelay = true;
                    var ips = await Dns.GetHostAddressesAsync(remoteServerIp);

                    new SCTcpClient(terminalClient, new IPEndPoint(ips.First(), remoteServerPort), latency, role);
                }
                catch (Exception ex) {
                    Logger.Log($"Exception: Faltal error starting or listening on port {localPort}: {ex.Message}. \nKeep listening ...");
                }
            }
        }
    }

    class SCTcpClient
    {
        private TcpClient _terminalClient;
        private IPEndPoint _clientEndpoint;
        private IPEndPoint _remoteServerEndPoint;
        private ushort _latency;
        private ProxyRole _role;
        Stopwatch stopWatch = new Stopwatch();

        public SCTcpClient(TcpClient terminalClient, IPEndPoint remoteServer, ushort latency, ProxyRole role)
        {
            _terminalClient = terminalClient;
            _latency = latency;
            _role = role;
            _remoteServerEndPoint = remoteServer;
            _remoteClient.NoDelay = true;
            _clientEndpoint = (IPEndPoint)_terminalClient.Client.RemoteEndPoint;
            Logger.Log($"Connection established from endpoint in {_clientEndpoint} -> redirecting to {remoteServer}");
            Run();
        }

        public void PrecisionSleep(int delay)
        {
            stopWatch.Reset();
            stopWatch.Start();

            // this line will create accurate 40 milisecond
            while (stopWatch.ElapsedMilliseconds < delay) { }
            stopWatch.Stop();
        }

        public TcpClient _remoteClient = new TcpClient();

        public Task TaskWrite(NetworkStream streamInput, SslStream streamOutput)
        {
            void writeAction()
            {
                var buffer = new byte[1024000];
                try
                {
                    for (; ; )
                    {
                        var readed = streamInput.Read(buffer);
                        if (readed == 0)
                        {
                            return;
                        }

                        // simulates latency
                        PrecisionSleep(_latency);
                        
                        streamOutput.Write(buffer, 0, readed);
                    }
                }
                catch
                {
                    return;
                }
            }

            return Task.Run(writeAction);
        }
        public Task TaskWriteSSL(SslStream streamInput, NetworkStream streamOutput)
        {
            void writeAction()
            {
                var buffer = new byte[1024000];
                try
                {
                    for (; ; )
                    {
                        var readed = streamInput.Read(buffer);
                        if (readed == 0)
                        {
                            return;
                        }

                        // simulates latency
                        PrecisionSleep(_latency);

                        streamOutput.Write(buffer, 0, readed);
                    }
                }
                catch
                {
                    return;
                }
            }

            return Task.Run(writeAction);
        }

        private void Run()
        {

            Task.Run((Func<Task>)(async () =>
            {
                try
                {
                    NetworkStream remoteStream = null;
                    NetworkStream terminalStream = null;
                    SslStream sslRemoteStream = null;
                    SslStream sslTerminalStream = null;

                    using (_terminalClient)
                    using (_remoteClient)
                    {
                        await _remoteClient.ConnectAsync(_remoteServerEndPoint.Address, _remoteServerEndPoint.Port);
                        if (_role == ProxyRole.Terminal) {
                            remoteStream = _remoteClient.GetStream();
                            sslRemoteStream = new SslStream(remoteStream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                            
                            try
                            {
                                sslRemoteStream.AuthenticateAsClient(_remoteServerEndPoint.Address.ToString(), null, SslProtocols.Tls12, true);
                            }
                            catch (Exception e)
                            {
                                Logger.Log($"Exception: Fatal error on client side in SSL handshake process: {e.Message}\n{e.InnerException}");
                                _remoteClient.Close();
                                return;
                            }
                            terminalStream = _terminalClient.GetStream();
                            await Task.WhenAny(sslRemoteStream.CopyToAsync(terminalStream), TaskWrite(terminalStream, sslRemoteStream));
                        } else if (_role == ProxyRole.Server) { 
                            terminalStream = _terminalClient.GetStream();
                            sslTerminalStream = new SslStream(terminalStream, false);

                            try
                            {
                                X509Certificate2 certificate = new X509Certificate2(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sccertificate.pfx"), "VisualTime#1");
                                sslTerminalStream.AuthenticateAsServer(certificate, false, SslProtocols.Tls12, true);
                            }
                            catch (Exception e)
                            {
                                Logger.Log($"Exception: Fatal error on server side in SSL handshake process: {e.Message}\n{e.InnerException}");
                                _remoteClient.Close();
                                return;
                            }
                            remoteStream = _remoteClient.GetStream();
                            await Task.WhenAny(remoteStream.CopyToAsync(sslTerminalStream), TaskWriteSSL(sslTerminalStream, remoteStream));
                        }
                    }
                }
                catch (Exception e) {
                    Logger.Log($"Exception: Fatal error on general SSL handshake process: {e.Message}\n{e.InnerException}");
                    return;
                }
                finally
                {
                    Logger.Log($"Connection closed between client at {_clientEndpoint} and remote at {_remoteServerEndPoint}");
                    _terminalClient = null;
                }
            }));
        }
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // No validation logic by the moment
            return true;
        }

    }
}
