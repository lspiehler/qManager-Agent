using System;
using System.ServiceModel.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Hosting.Wcf;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy;
using Nancy.Hosting.Self;
using System.ServiceModel;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace PrintManagement
{

    /*public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            // your customization goes here
            StaticConfiguration.DisableErrorTraces = false;
            var hostConf = new HostConfiguration();
            hostConf.EnableClientCertificates = true;
        }

        protected override IRootPathProvider RootPathProvider
        {
            get { return new CustomRootPathProvider(); }
        }
    }

    public class CustomRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return Directory.GetCurrentDirectory();
        }
    }*/

    public class WindowsService
    {
        //private string _url = "http://127.0.0.1";
        //private int _port = 8080;
        //private NancyHost _nancy;
        //private WebServiceHost _host;
        wsClient wsclient = new wsClient();
        
        public WindowsService()
        {
            //var builder = WebApplication.CreateBuilder(args);

            //var uri = new Uri($"{_url}:{_port}/");
            //_nancy = new NancyHost(uri);

            //_host = new WebServiceHost(new NancyWcfGenericService(), uri);

            //var _host = new WebServiceHost(new NancyWcfGenericService(new DefaultNancyBootstrapper()), uri);

            //var binding = new WebHttpBinding();
            //binding.Security.Mode = WebHttpSecurityMode.Transport;
            //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

            //_host.AddServiceEndpoint(typeof(NancyWcfGenericService), binding, "");

            //_host.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, "ea 45 c4 ad c1 2e 8e 2d e8 c5 92 c6 ef 66 0c 05 57 19 18 e0");
        }

        public void Start()
        {
            //_nancy.Start();
            wsclient.initiateWebSocket();
            //_host.Open();
        }

        public void Stop()
        {
            //_nancy.Stop();
            wsclient.closeSocket();
            //_host.Close();
        }

        /*static void Main(string[] args)
        {
            var p = new WebService();
            p.Start();
        }*/
    }
}
