using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Management.Automation;
using System.Management;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace PrintManagement.pslib
{
    class printPort
    {

        static managementScope ms = managementScope.Instance;
        static configHandler confighandler = configHandler.Instance;
        static Dictionary<string, string> config = confighandler.getConfig();

        Dictionary<string, printerlib.GetPrinterPort> cachedobjects;

        public ManagementObject Get(string name)
        {
            try
            {
                ManagementObject printport = new ManagementObject(ms.Get(), new ManagementPath(@"Win32_TCPIPPrinterPort.Name='" + name + "'"), new ObjectGetOptions());
                printport.Get();
                if (printport["Name"].ToString().ToLower() == name.ToLower())
                {
                    return printport;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }

        public string Create(string name, int protocol, string address, int port, bool snmp)
        {
            /*if (CheckPrinterPort())
                return true;*/
            try
            {
                ManagementObject printport = Get(name);
                if (printport == null)
                {
                    var printerPortClass = new ManagementClass(ms.Get(), new ManagementPath("Win32_TCPIPPrinterPort"), new ObjectGetOptions());
                    printerPortClass.Get();
                    var newPrinterPort = printerPortClass.CreateInstance();
                    newPrinterPort.SetPropertyValue("Name", name);
                    newPrinterPort.SetPropertyValue("Protocol", protocol);
                    newPrinterPort.SetPropertyValue("HostAddress", address);
                    newPrinterPort.SetPropertyValue("PortNumber", port);    // default=9100
                    newPrinterPort.SetPropertyValue("SNMPEnabled", snmp);  // true?
                    newPrinterPort.Put();

                    ManagementObject printerport = Get(name);
                    if (printerport != null)
                    {
                        if (config["Script"] != null)
                        {
                            if (File.Exists(config["Script"]))
                            {
                                printerlib.PSPortResult psportresult = new printerlib.PSPortResult();
                                psportresult.Action = "Create";
                                psportresult.Type = "Port";
                                printerlib.GetPrinterPort getobject = new printerlib.GetPrinterPort();
                                foreach (PropertyData prop in printerport.Properties)
                                {
                                    var getproperty = getobject.GetType().GetProperty(prop.Name);
                                    if (getproperty != null)
                                    {
                                        if (prop.Value == null)
                                        {
                                            getproperty.SetValue(getobject, null);
                                        }
                                        else
                                        {
                                            getproperty.SetValue(getobject, prop.Value);
                                        }
                                    }
                                    //Console.WriteLine("{0}: {1}", prop.Name, prop.Value);
                                }
                                psportresult.Port = getobject;
                                string jsonresult = JsonConvert.SerializeObject(psportresult);
                                using (System.Management.Automation.PowerShell PowerShellInst = System.Management.Automation.PowerShell.Create())
                                {
                                    PowerShellInst.AddScript("$json = '" + jsonresult + "' | ConvertFrom-Json\r\n. \"" + config["Script"] + "\" -result $json");
                                    //PowerShellInst.AddScript("$json = '"+ jsonprinter +"' | ConvertFrom-Json");
                                    //PowerShellInst.AddCommand(". \"C:\\Users\\LyasSpiehler\\OneDrive - Sapphire Health Technology Services\\Desktop\\powershell test\\script.ps1\" -Result $json");

                                    Collection<System.Management.Automation.PSObject> PSOutput = PowerShellInst.Invoke();

                                    if (PowerShellInst.HadErrors)
                                    {
                                        List<string> errors = new List<string>();
                                        for (int i = 0; i < PowerShellInst.Streams.Error.Count; i++)
                                        {
                                            errors.Add(PowerShellInst.Streams.Error[i].ToString());
                                        }
                                        errorlog el = new errorlog();
                                        el.write("Error executing script " + config["Script"] + ": " + String.Join("", errors), Environment.StackTrace, "error");
                                        return "Error executing script " + config["Script"] + ": " + String.Join("", errors);
                                    }
                                    else
                                    {
                                        return null;
                                    }
                                }
                            }
                            else
                            {
                                return config["Script"] + " does not exist";
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return "Failed to create the port " + name;
                    }
                } else
                {
                    return "The printer port already exists";
                }
            }
            catch (Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");
                return e.ToString();
            }

            return null;
        }

        public Dictionary<string, printerlib.GetPrinterPort> CIMGetAll(bool updatecache)
        {
            //https://stackoverflow.com/questions/978862/how-to-make-forward-only-read-only-wmi-queries-in-c
            if (updatecache == true || cachedobjects == null)
            {
                try
                {
                    Dictionary<string, printerlib.GetPrinterPort> objects = new Dictionary<string, printerlib.GetPrinterPort>();

                    //System.Management.ManagementObject oq = new System.Management.ObjectQuery("Select * from Win32_Printer");
                    EnumerationOptions options = new EnumerationOptions();
                    options.Rewindable = false;
                    options.ReturnImmediately = true;

                    SelectQuery query = new SelectQuery("Select * From Win32_TCPIPPrinterPort");
                    ManagementObjectSearcher printerports = new ManagementObjectSearcher(ms.Get(), query, options);

                    foreach (ManagementObject printerport in printerports.Get())
                    {
                        printerlib.GetPrinterPort getobject = new printerlib.GetPrinterPort();
                        foreach (PropertyData prop in printerport.Properties)
                        {
                            var getproperty = getobject.GetType().GetProperty(prop.Name);
                            if (getproperty != null)
                            {
                                if (prop.Value == null)
                                {
                                    getproperty.SetValue(getobject, null);
                                }
                                else
                                {
                                    getproperty.SetValue(getobject, prop.Value);
                                }
                            }
                            //Console.WriteLine("{0}: {1}", prop.Name, prop.Value);
                        }

                        objects.Add(printerport.Properties["Name"].Value.ToString(), getobject);
                        getobject.PrinterHostAddress = (string)printerport.Properties["HostAddress"].Value;
                    }

                    cachedobjects = objects;
                    return objects;
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                    return null;
                }
            }
            else
            {
                //Console.WriteLine("Returning cached printers");
                return cachedobjects;
            }
        }
        public Dictionary<string, printerlib.GetPrinterPort> GetAll(bool updatecache)
        {
            if (config["Legacy"] != null && config["Legacy"].ToLower() == "true")
            {
                return CIMGetAll(updatecache);
            }
            else
            {
                if (updatecache == true || cachedobjects == null)
                {
                    Dictionary<string, printerlib.GetPrinterPort> objects = new Dictionary<string, printerlib.GetPrinterPort>();

                    try
                    {
                        using (System.Management.Automation.PowerShell PowerShellInst = System.Management.Automation.PowerShell.Create())
                        {
                            /*if (config["Legacy"] != null && config["Legacy"].ToLower() == "true")
                            {
                                Console.WriteLine("using legacy command");
                                PowerShellInst.AddCommand("Get-WmiObject").AddParameter("Query", "SELECT * FROM Win32_TCPIPPrinterPort");
                            }
                            else
                            {*/
                            PowerShellInst.AddCommand("Get-PrinterPort");
                            //}
                            Collection<System.Management.Automation.PSObject> PSOutput = PowerShellInst.Invoke();

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

                            foreach (System.Management.Automation.PSObject obj in PSOutput)
                            {
                                printerlib.GetPrinterPort getobject = new printerlib.GetPrinterPort();
                                if (obj != null)
                                {
                                    List<System.Management.Automation.PSPropertyInfo> properties = obj.Properties.ToList();
                                    for (int i = 0; i < properties.Count; i++)
                                    {
                                        System.Management.Automation.PSPropertyInfo psobject = obj.Properties[properties[i].Name];
                                        if (psobject.GetType().ToString() == "System.Management.Automation.PSAdaptedProperty" || psobject.GetType().ToString() == "System.Management.Automation.PSProperty")
                                        {
                                            if (properties[i].Name.IndexOf("_") != 0)
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
                                }

                                //add custom mappings if legacy commands are being used
                                /*if (config["Legacy"] != null && config["Legacy"].ToLower() == "true")
                                {
                                    getobject.PrinterHostAddress = (string)obj.Properties["HostAddress"].Value;
                                }*/

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
                    //Console.WriteLine("Used cached powershell command");
                    return cachedobjects;
                }
            }
        }
    }
}
