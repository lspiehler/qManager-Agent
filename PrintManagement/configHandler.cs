using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Newtonsoft.Json;

namespace PrintManagement
{

    public class Options
    {
        [Option('p', "proxy", Required = false, HelpText = "Use a proxy for the connection to qManager")]
        public string Proxy { get; set; }

        [Option('l', "legacy", Required = false, HelpText = "Use a proxy for the connection to qManager")]
        public string Legacy { get; set; }

        [Option('s', "server", Required = false, HelpText = "The FQDN for a qManager server")]
        public string Server { get; set; }

        [Option('c', "cert", Required = false, HelpText = "The certificate used for authentication to qManager")]
        public string Certificate { get; set; }

        [Option('m', "showpings", Required = false, HelpText = "Show logs of ping requests from the server. This can be helpful for troubleshooting.")]
        public string ShowPings { get; set; }

        [Option('g', "groups", Required = false, HelpText = "Comma separated list of groups the agent will be a member of")]
        public string Groups { get; set; }

        [Option('e', "script", Required = false, HelpText = "Script to call when printers are created or modified")]
        public string Script { get; set; }
    }
    public sealed class configHandler
    {
        private static readonly configHandler instance = new configHandler();
        private static Dictionary<string, string> config;

        static configHandler()
        {
        }
        private configHandler()
        {
        }
        public static configHandler Instance
        {
            get
            {
                return instance;
            }
        }

        public Dictionary<string, string> getConfig()
        {
            return config;
        }
        public void loadConfig(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       /*if (o.Proxy == null)
                       {
                           //Console.WriteLine($"Verbose output enabled. Current Arguments: -v {o.Verbose}");
                           //Console.WriteLine("Quick Start Example! App is in Verbose mode!");
                           Console.WriteLine("Proxy is null");
                       }
                       else
                       {
                           //Console.WriteLine($"Current Arguments: -v {o.Verbose}");
                           //Console.WriteLine("Quick Start Example!");
                           Console.WriteLine(o.Proxy);
                       }*/
                       config = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(o));
                   });
        }
    }
}
