using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement
{
    public partial class AgentService : ServiceBase
    {

        WindowsService instance = new WindowsService();
        public AgentService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            instance.Start();
        }

        protected override void OnStop()
        {
            instance.Stop();
        }

        /*public void RunAsConsole(string[] args)
        {
            OnStart(args);
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            WebService instance = new WebService();
            instance.Samplemodule();
            OnStop();
        }*/
    }
}
