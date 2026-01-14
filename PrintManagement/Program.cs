using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.IO;

namespace PrintManagement
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        //[STAThread]
        static void Main(string[] args)
        {
            configHandler confighandler = configHandler.Instance;
            confighandler.loadConfig(args);
            Dictionary<string, string> config = confighandler.getConfig();
            if (config["Script"] != null)
            {
                if (!File.Exists(config["Script"]))
                {
                    errorlog el = new errorlog();
                    el.write(config["Script"] + " does not exist", Environment.StackTrace, "error");
                    Console.WriteLine(config["Script"] + " does not exist");
                    return;
                }
            }
            if (Environment.UserInteractive)
            {
                wsClient wsclient = new wsClient();
                wsclient.initiateWebSocket();

                /*Console.WriteLine("Opening socket");
                Console.WriteLine("Starting HTTP listener...");

                //var httpServer = new HttpServer();
                //httpServer.Start();
                WebService instance = new WebService();
                instance.Start();*/

                var autoResetEvent = new AutoResetEvent(false);
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    // cancel the cancellation to allow the program to shutdown cleanly
                    eventArgs.Cancel = true;
                    autoResetEvent.Set();
                };

                // main blocks here waiting for ctrl-C
                autoResetEvent.WaitOne();
                Console.WriteLine("Now shutting down");

                //wsclient.getSocket().Abort();
                //if (wsclient.getState() == "Open") {
                    wsclient.closeSocket();
                //}

                //while (Program._keepRunning) { }

                //httpServer.Stop();
                //instance.Stop();

                //Console.WriteLine("Exiting gracefully...");
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new AgentService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
