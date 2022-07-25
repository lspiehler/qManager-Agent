using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.pslib
{
    class getPrinterPort
    {
        static configHandler confighandler = configHandler.Instance;
        static Dictionary<string, string> config = confighandler.getConfig();
        public class responseBody
        {
            public string result { get; set; }
            public string message { get; set; }
            public Dictionary<string, printerlib.GetPrinterPort> data { get; set; }
        }

        Dictionary<string, printerlib.GetPrinterPort> cachedobjects;
        public Dictionary<string, printerlib.GetPrinterPort> Run(bool updatecache)
        {
            if (updatecache == true || cachedobjects == null)
            {
                Dictionary<string, printerlib.GetPrinterPort> objects = new Dictionary<string, printerlib.GetPrinterPort>();

                try
                {
                    using (PowerShell PowerShellInst = PowerShell.Create())
                    {
                        if (config["Legacy"] != null && config["Legacy"].ToLower() == "true")
                        {
                            Console.WriteLine("using legacy command");
                            PowerShellInst.AddCommand("Get-WmiObject").AddParameter("Query", "SELECT * FROM Win32_TCPIPPrinterPort");
                        }
                        else
                        {
                            PowerShellInst.AddCommand("Get-PrinterPort");
                        }
                        Collection<PSObject> PSOutput = PowerShellInst.Invoke();

                        if (PowerShellInst.HadErrors)
                        {
                            List<string> errors = new List<string>();
                            for (int i = 0; i < PowerShellInst.Streams.Error.Count; i++)
                            {
                                errors.Add(PowerShellInst.Streams.Error[i].ToString());
                            }
                            errorlog el = new errorlog();
                            el.write(String.Join("", errors), Environment.StackTrace, "error");
                            return objects;
                        }

                        foreach (PSObject obj in PSOutput)
                        {
                            printerlib.GetPrinterPort getobject = new printerlib.GetPrinterPort();
                            if (obj != null)
                            {
                                List<PSPropertyInfo> properties = obj.Properties.ToList();
                                for (int i = 0; i < properties.Count; i++)
                                {
                                    PSPropertyInfo psobject = obj.Properties[properties[i].Name];
                                    if (psobject.GetType().ToString() == "System.Management.Automation.PSAdaptedProperty")
                                    {
                                        var getproperty = getobject.GetType().GetProperty(properties[i].Name);
                                        if (getproperty != null)
                                        {
                                            if (psobject.Value == null)
                                            {
                                                getproperty.SetValue(getobject, null);
                                            }
                                            else
                                            {
                                                getproperty.SetValue(getobject, psobject.Value);
                                            }
                                        }
                                    }
                                }
                            }

                            //add custom mappings if legacy commands are being used
                            if (config["Legacy"] != null && config["Legacy"].ToLower() == "true")
                            {
                                getobject.PrinterHostAddress = (string)obj.Properties["HostAddress"].Value;
                            }

                            objects.Add(obj.Properties["Name"].Value.ToString(), getobject);
                        }
                        cachedobjects = objects;
                        return objects;
                    }
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), "", "error");
                    return objects;
                }
            }
            else
            {
                Console.WriteLine("Used cached powershell command");
                return cachedobjects;
            }
        }
    }
}
