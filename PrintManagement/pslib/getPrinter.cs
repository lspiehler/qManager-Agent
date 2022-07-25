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
    class getPrinter
    {
        static configHandler confighandler = configHandler.Instance;
        static Dictionary<string, string> config = confighandler.getConfig();

        Dictionary<string, printerlib.GetPrinter> cachedobjects;
        public Dictionary<string, printerlib.GetPrinter> Run(bool updatecache)
        {
            if (updatecache == true || cachedobjects == null) {
                Console.WriteLine("Running powershell command");

                getPrinterPort getprinterport = new getPrinterPort();
                Dictionary<string, printerlib.GetPrinterPort> printerports = getprinterport.Run(updatecache);

                Dictionary<string, printerlib.GetPrinter> objects = new Dictionary<string, printerlib.GetPrinter>();
                try
                {
                    using (PowerShell PowerShellInst = PowerShell.Create())
                    {
                        if (config["Legacy"] != null && config["Legacy"].ToLower() == "true")
                        {
                            Console.WriteLine("using legacy command");
                            PowerShellInst.AddCommand("Get-CimInstance").AddParameter("ClassName", "CIM_Printer");
                        }
                        else
                        {
                            PowerShellInst.AddCommand("Get-Printer");
                        }

                        Collection<PSObject> PSOutput = PowerShellInst.Invoke();

                        if(PowerShellInst.HadErrors)
                        {
                            List<string> errors = new List<string>();
                            for(int i = 0; i < PowerShellInst.Streams.Error.Count; i++)
                            {
                                errors.Add(PowerShellInst.Streams.Error[i].ToString());
                            }
                            errorlog el = new errorlog();
                            el.write(String.Join("", errors), Environment.StackTrace, "error");
                            return objects;
                        }

                        foreach (PSObject obj in PSOutput)
                        {
                            printerlib.GetPrinter getobject = new printerlib.GetPrinter();
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

                            //add port address
                            if (printerports.ContainsKey(obj.Properties["PortName"].Value.ToString()))
                            {
                                getobject.PrinterHostAddress = printerports[obj.Properties["PortName"].Value.ToString()].PrinterHostAddress;
                            }

                            //add custom mappings if legacy commands are being used
                            if (config["Legacy"] != null && config["Legacy"].ToLower() == "true")
                            {
                                getobject.JobCount = (uint)obj.Properties["JobCountSinceLastReset"].Value;
                                getobject.PrinterStatus = (uint)obj.Properties["PrinterState"].Value;
                            }

                            objects.Add(obj.Properties["Name"].Value.ToString(), getobject);
                        }
                        cachedobjects = objects;
                        return objects;
                    }
                }
                catch(Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                    return objects;
                }
            } else
            {
                Console.WriteLine("Used cached powershell command");
                return cachedobjects;
            }
        }
    }
}
