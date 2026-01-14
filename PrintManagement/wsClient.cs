using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
//using System.Net.NetworkInformation;


namespace PrintManagement
{
    class wsClient
    {

        System.Timers.Timer timer = new System.Timers.Timer();
        static configHandler confighandler = configHandler.Instance;
        static Dictionary<string, string> config = confighandler.getConfig();

        private void timerElapsed(object sender, EventArgs ew)
        {
            if (connected) {
                clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                connected = false;
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString() + " The connection to the server failed. Trying again...");
                ctoken.Cancel();
            }
            Console.WriteLine(DateTime.Now.ToString() + " Timer expired");
            Console.WriteLine(DateTime.Now.ToString() + " " + clientWebSocket.State.ToString());
            clientWebSocket.Dispose();
            ctoken.Dispose();
            //clientWebSocket = new System.Net.WebSockets.Managed.ClientWebSocket();
            ctoken = new CancellationTokenSource();
            initiateWebSocket();
        }

        private void processTimer()
        {
            if (timer.Enabled) {
                //Console.WriteLine("Timer interrupted");
                timer.Stop();
                timer.Start();
            }

            if(timerinitiated==false)
            {
                timer.Interval = 15000;
                timer.AutoReset = false;
                timer.Elapsed += timerElapsed;
                timer.Start();
                timerinitiated = true;
            }
            else
            {
                timer.Start();
            }
        }

        private System.Net.WebSockets.Managed.ClientWebSocket clientWebSocket;
        private CancellationTokenSource ctoken = new CancellationTokenSource();
        private bool connected = false;
        private bool timerinitiated = false;
        //private CancellationToken token = CancellationToken.None;
        wsHandler wsrouter = new wsHandler();

        /*private static string GetLocalhostFqdn()
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            return string.IsNullOrWhiteSpace(ipProperties.DomainName) ? ipProperties.HostName : string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName);
        }*/
        public void closeSocket()
        {
            if (timer.Enabled)
            {
                timer.Stop();
            }
            if (connected)
            {
                if (clientWebSocket.State.ToString() == "Open") {
                    clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                }
                else
                {
                    clientWebSocket.Dispose();
                }
            }
            else
            {
                if(ctoken != null && ctoken.Token.CanBeCanceled)
                {
                    ctoken.Cancel();
                    ctoken.Dispose();
                }
                if (clientWebSocket != null)
                {
                    clientWebSocket.Dispose();
                }
            }
        }

        public string getState()
        {
            return clientWebSocket.State.ToString();
        }
        public async Task initiateWebSocket()
        {
            var location = new Uri("wss://" + config["Server"] + "/qmanager");

            /*string certificatePath = "C:\\win-print-api\\cert\\cert.pfx";

            string certificatePassword = "1234";

            X509Certificate clientCertificate = new X509Certificate2(certificatePath, certificatePassword);*/

            List<StoreLocation> locations = new List<StoreLocation>();
            locations.Add(StoreLocation.LocalMachine);
            locations.Add(StoreLocation.CurrentUser);

            X509Certificate2 certificate = null;

            for (int i = 0; i < locations.Count; i++)
            {
                X509Store store = new X509Store(StoreName.My, locations[i]);

                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                X509Certificate2Collection col = store.Certificates.Find(X509FindType.FindBySubjectName, config["Certificate"], false);

                for (int j = 0; j < col.Count; j++)
                {
                    if (col[j].HasPrivateKey)
                    {
                        if (col[j].NotBefore <= DateTime.Now)
                        {
                            if (col[j].NotAfter >= DateTime.Now)
                            {
                                if (certificate == null)
                                {
                                    certificate = col[j];
                                }
                                else
                                {
                                    if (col[j].NotAfter <= certificate.NotAfter)
                                    {
                                        certificate = col[j];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            clientWebSocket = new System.Net.WebSockets.Managed.ClientWebSocket();

            if (certificate == null) {
                errorlog el = new errorlog();
                el.write("Failed to find certificate", Environment.StackTrace, "error");
                //Console.WriteLine("here");
                Console.ReadLine();
                Environment.Exit(-1);
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString() + " Using certificate:");
                Console.WriteLine(certificate.ToString());
                clientWebSocket.Options.ClientCertificates.Add(certificate);
            }

            if (config["Proxy"] != null) {
                Console.WriteLine(DateTime.Now.ToString() + " Using proxy " + config["Proxy"]);
                System.Net.WebProxy proxy = new System.Net.WebProxy(config["Proxy"]);

                clientWebSocket.Options.Proxy = proxy;
            }

            Console.WriteLine(DateTime.Now.ToString() + " Opening web socket");

            processTimer();

            try {
                await clientWebSocket.ConnectAsync(location, ctoken.Token);

            }
            catch(Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");
                Console.WriteLine(DateTime.Now.ToString() + " " + e.ToString());
            }

            //Console.WriteLine("done");

            if (clientWebSocket.State == WebSocketState.Open)
            {
                connected = true;

                byte[] buffer = new byte[32768];

                while (clientWebSocket.State == WebSocketState.Open)
                {
                    MemoryStream ms = new MemoryStream();
                    WebSocketReceiveResult result = null;

                    try
                    {
                        do
                        {
                            result = await clientWebSocket.ReceiveAsync(
                                new ArraySegment<byte>(buffer),
                                CancellationToken.None);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await clientWebSocket.CloseAsync(
                                    WebSocketCloseStatus.NormalClosure,
                                    "Closing",
                                    CancellationToken.None);
                                return;
                            }

                            ms.Write(buffer, 0, result.Count);

                        } while (!result.EndOfMessage);

                        processTimer();

                        // IMPORTANT: copy before handing off
                        byte[] messageBytes = ms.ToArray();

                        // Sequential, safe processing
                        await wsrouter.wsRequestHandler(
                            clientWebSocket,
                            messageBytes,
                            result.MessageType
                        );
                    }
                    finally
                    {
                        ms.Dispose();
                    }
                }
            }


        }

    }
}
