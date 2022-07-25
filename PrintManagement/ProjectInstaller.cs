using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace PrintManagement
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            new ServiceController(serviceInstaller1.ServiceName).Start();
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceInstaller1_BeforeUninstall(object sender, InstallEventArgs e)
        {
            System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(serviceInstaller1.ServiceName);

            if (controller.Status == System.ServiceProcess.ServiceControllerStatus.Running | controller.Status == System.ServiceProcess.ServiceControllerStatus.Paused)

            {

                controller.Stop();

                controller.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 0, 15));

                controller.Close();

            }

            base.OnBeforeUninstall(e.SavedState);
        }
    }
}
