using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Printing;
using System.Management.Automation;
using System.Collections.ObjectModel;

namespace PrintManagement.pslib
{
    class printQueue
    {
        static managementScope ms = managementScope.Instance;
        static configHandler confighandler = configHandler.Instance;
        static Dictionary<string, string> config = confighandler.getConfig();
        Dictionary<string, printerlib.GetPrinter> cachedobjects;
        getPrinterPort getprinterport = new getPrinterPort();
        printPort printport = new printPort();

        Dictionary<string, Dictionary<string, string>> printprops = new Dictionary<string, Dictionary<string, string>>
        {
            { "name", new Dictionary<string, string>
                {
                    { "type", "string" },
                    { "name", "Name" }
                }
            },
            { "portname", new Dictionary<string, string>
                {
                    { "type", "string" },
                    { "name", "PortName" }
                }
            },
            { "comment", new Dictionary<string, string>
                {
                    { "type", "string" },
                    { "name", "Comment" }
                }
            },
            { "location", new Dictionary<string, string>
                {
                    { "type", "string" },
                    { "name", "Location" }
                }
            },
            { "shared", new Dictionary<string, string>
                {
                    { "type", "bool" },
                    { "name", "Shared" }
                }
            },
            { "sharename", new Dictionary<string, string>
                {
                    { "type", "bool" },
                    { "name", "Shared" }
                }
            },
            { "drivername", new Dictionary<string, string>
                {
                    { "type", "string" },
                    { "name", "DriverName" }
                }
            },
            { "published", new Dictionary<string, string>
                {
                    { "type", "bool" },
                    { "name", "Published" }
                }
            },

        };

        Dictionary<string, Dictionary<string, string>> queueprops = new Dictionary<string, Dictionary<string, string>>
        {
            { "DriverName", new Dictionary<string, string>
                {
                    { "type", "string" },
                    { "name", "DriverName" }
                }
            },
            { "PortName", new Dictionary<string, string>
                {
                    { "type", "string" },
                    { "name", "PortName" }
                }
            },
            { "Comment", new Dictionary<string, string>
                {
                    { "type", "string" },
                    { "name", "Comment" }
                }
            },
            { "Location", new Dictionary<string, string>
                {
                    { "type", "string" },
                    { "name", "Location" }
                }
            }
        };
        
        private void updateCache(string queuename)
        { // need to modify code for updating vs creating. Check if key exists in cache????
            Console.WriteLine("Updating cache for queue name " + queuename);
            try
            {
                ManagementObject printer = Get(queuename);
                if (printer != null)
                {
                    printerlib.GetPrinter getobject = new printerlib.GetPrinter();

                    foreach (PropertyData prop in printer.Properties)
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

                    ManagementObject port = printport.Get(printer.Properties["PortName"].Value.ToString());
                    if (port != null)
                    {
                        getobject.PrinterHostAddress = port.Properties["HostAddress"].Value.ToString();
                        getobject.JobCount = (uint)printer.Properties["JobCountSinceLastReset"].Value;
                        getobject.PrinterStatus = (uint)printer.Properties["PrinterState"].Value;

                        //Console.WriteLine("Cache updated successfully");

                        if(cachedobjects.ContainsKey(queuename))
                        {
                            cachedobjects[queuename] = getobject;
                        }
                        else
                        {
                            cachedobjects.Add(queuename, getobject);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to find the port to update the cache");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to find the printer to update the cache");
                }
            }
            catch(Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");
                return;
            }
        }

        public Dictionary<string, printerlib.GetPrinter> WMIGetAll(bool updatecache)
        {
            //https://stackoverflow.com/questions/978862/how-to-make-forward-only-read-only-wmi-queries-in-c
            if (updatecache == true || cachedobjects == null)
            {
                try
                {
                    Dictionary<string, printerlib.GetPrinter> objects = new Dictionary<string, printerlib.GetPrinter>();
                    Dictionary<string, printerlib.GetPrinterPort> printerports = getprinterport.Run(updatecache);

                    //System.Management.ManagementObject oq = new System.Management.ObjectQuery("Select * from Win32_Printer");
                    EnumerationOptions options = new EnumerationOptions();
                    options.Rewindable = false;
                    options.ReturnImmediately = true;

                    SelectQuery query = new SelectQuery("Select * From Win32_Printer");
                    ManagementObjectSearcher printers = new ManagementObjectSearcher(ms.Get(), query, options);

                    foreach (ManagementObject printer in printers.Get())
                    {
                        printerlib.GetPrinter getobject = new printerlib.GetPrinter();
                        foreach (PropertyData prop in printer.Properties)
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
                        //Console.WriteLine(printer["Name"]);
                        if (printerports.ContainsKey(printer.Properties["PortName"].Value.ToString()))
                        {
                            getobject.PrinterHostAddress = printerports[printer.Properties["PortName"].Value.ToString()].PrinterHostAddress;
                        }

                        getobject.JobCount = (uint)printer.Properties["JobCountSinceLastReset"].Value;
                        getobject.PrinterStatus = (uint)printer.Properties["PrinterState"].Value;

                        objects.Add(printer.Properties["Name"].Value.ToString(), getobject);
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
                Console.WriteLine("Returning cached printers");
                return cachedobjects;
            }
        }

        public Dictionary<string, printerlib.GetPrinter> CIMGetAll(bool updatecache)
        {
            // convert this to CIM instead of WMI
            //https://stackoverflow.com/questions/978862/how-to-make-forward-only-read-only-wmi-queries-in-c
            if (updatecache == true || cachedobjects == null)
            {
                try
                {
                    Dictionary<string, printerlib.GetPrinter> objects = new Dictionary<string, printerlib.GetPrinter>();
                    Dictionary<string, printerlib.GetPrinterPort> printerports = getprinterport.Run(updatecache);

                    //System.Management.ManagementObject oq = new System.Management.ObjectQuery("Select * from Win32_Printer");
                    EnumerationOptions options = new EnumerationOptions();
                    options.Rewindable = false;
                    options.ReturnImmediately = true;

                    SelectQuery query = new SelectQuery("Select * From Win32_Printer");
                    ManagementObjectSearcher printers = new ManagementObjectSearcher(ms.Get(), query, options);

                    foreach (ManagementObject printer in printers.Get())
                    {
                        printerlib.GetPrinter getobject = new printerlib.GetPrinter();
                        foreach (PropertyData prop in printer.Properties)
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
                        //Console.WriteLine(printer["Name"]);
                        if (printerports.ContainsKey(printer.Properties["PortName"].Value.ToString()))
                        {
                            getobject.PrinterHostAddress = printerports[printer.Properties["PortName"].Value.ToString()].PrinterHostAddress;
                        }

                        getobject.JobCount = (uint)printer.Properties["JobCountSinceLastReset"].Value;
                        getobject.PrinterStatus = (uint)printer.Properties["PrinterState"].Value;

                        objects.Add(printer.Properties["Name"].Value.ToString(), getobject);
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
                Console.WriteLine("Returning cached printers");
                return cachedobjects;
            }
        }

        public Dictionary<string, printerlib.GetPrinter> GetAll(bool updatecache)
        {
            if (updatecache == true || cachedobjects == null)
            {
                Console.WriteLine("Running powershell command");

                //getPrinterPort getprinterport = new getPrinterPort();
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

                        if (PowerShellInst.HadErrors)
                        {
                            List<string> errors = new List<string>();
                            for (int i = 0; i < PowerShellInst.Streams.Error.Count; i++)
                            {
                                errors.Add(PowerShellInst.Streams.Error[i].ToString());
                            }
                            errorlog el = new errorlog();
                            el.write(String.Join("", errors), Environment.StackTrace, "error");
                            return null;
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
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Used cached powershell command");
                return cachedobjects;
            }
        }
        public string Clear(string name)
        {
            try
            {
                PrintQueue printqueue = new PrintQueue(new LocalPrintServer(), name, PrintSystemDesiredAccess.AdministratePrinter);
                printqueue.Purge();
            }
            catch(Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");

                return e.ToString();
            }
            return null;
        }
        public ManagementObject Get(string name)
        {
            try
            {
                ManagementObject printqueue = new ManagementObject(ms.Get(), new ManagementPath(@"win32_printer.DeviceId='" + name + "'"), new ObjectGetOptions());
                printqueue.Get();
                if (printqueue["DeviceId"].ToString().ToLower() == name.ToLower())
                {
                    return printqueue;
                } else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }

        public string Delete(string name)
        {
            //Console.WriteLine("function called");
            /*if (CheckPrinterPort())
                return true;*/

            //wmi method
            try
            {
                ManagementObject printqueue = Get(name);
                if (printqueue != null)
                {
                    /*bool result = LocalPrintServer.DeletePrintQueue(name);
                    if (result)
                    {
                        return null;
                    }
                    else
                    {
                        return "Failed to delete the print queue";
                    }*/
                    printqueue.Delete();
                    cachedobjects.Remove(name);
                    return null;
                } else
                {
                    return "The printer " + name + " does not exist";
                }
            }
            catch (Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");
                return e.ToString();
            }

            //return null;
        }

        public string Create(Dictionary<string, dynamic> printoptions)
        {
            //Console.WriteLine("function called");
            /*if (CheckPrinterPort())
                return true;*/

            //wmi method
            try
            {
                ManagementObject printqueue = Get(printoptions["name"]);
                if (printqueue == null)
                {
                    //var printerClass = new ManagementClass(ms.Get(), new ManagementPath("Win32_Printer"), null);
                    //printerClass.Get();
                    //var printer = printerClass.CreateInstance();
                    //ManagementBaseObject printer = printerClass.GetMethodParameters("AddPrinterConnection");
                    var printerClass = new ManagementClass(ms.Get(), new ManagementPath("Win32_Printer"), new ObjectGetOptions());
                    printerClass.Get();
                    var printer = printerClass.CreateInstance();
                    var props = printoptions.Keys;
                    /*Console.WriteLine("Requested printer properties:");
                    foreach (var p in props)
                    {
                        Console.WriteLine(p + ": " + printoptions[p]);
                    }*/
                    foreach (var p in props)
                    {
                        //Console.WriteLine(p);
                        //Console.WriteLine(printoptions[p]);
                        if (printprops.ContainsKey(p))
                        {
                            if (printprops[p]["type"] == "bool")
                            {
                                printer.SetPropertyValue(printprops[p]["name"], printoptions[p]);
                                //Console.WriteLine(printprops[p]["name"] + " = " + printoptions[p] + " - " + printoptions[p].GetType().ToString());
                            }
                            else if (printprops[p]["type"] == "int")
                            {
                                printer.SetPropertyValue(printprops[p]["name"], printoptions[p]);
                                //Console.WriteLine(printprops[p]["name"] + " = " + printoptions[p] + " - " + printoptions[p].GetType().ToString());
                            }
                            else
                            {
                                printer.SetPropertyValue(printprops[p]["name"], printoptions[p]);
                                //Console.WriteLine(printprops[p]["name"] + " = " + printoptions[p] + " - " + printoptions[p].GetType().ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine("Failed to set unknown property " + p + " while creating print queue " + printoptions["name"]);
                        }
                    }
                    printer.SetPropertyValue("Network", true);
                    printer.SetPropertyValue("Default", false);
                    printer.SetPropertyValue("DeviceID", printoptions["name"]);
                    if (printoptions.ContainsKey("sharename") == false)
                    {
                        printer.SetPropertyValue("ShareName", printoptions["name"]);
                    }
                    //printer.SetPropertyValue("WorkOffline", true);

                    var npp = printer.Properties;
                    /*foreach (var p in npp)
                    {
                        Console.WriteLine(p.Name + ": " + printer.GetPropertyValue(p.Name));
                    }*/

                    //printer.SetPropertyValue("DriverName", "Canon MF731C/733C UFR II");
                    //printer.SetPropertyValue("PortName", "192.168.1.52");
                    //printer.SetPropertyValue("Name", "TestPrinter55");
                    //printer.SetPropertyValue("DeviceID", "TestPrinter55");
                    //printer.SetPropertyValue("Location", "");
                    //printer.SetPropertyValue("Network", true);
                    //printer.SetPropertyValue("Shared", true);
                    //printer.SetPropertyValue("Default", true);
                    //ManagementBaseObject result = printerClass.InvokeMethod("AddPrinterConnection", printer, null);
                    PutOptions options = new PutOptions();
                    //options.Type = PutType.UpdateOrCreate;
                    options.Type = PutType.CreateOnly;
                    printer.Put(options);

                    updateCache(printoptions["name"]);

                    return null;
                }
                else
                {
                    return "A printer named " + printoptions["name"] + " already exists";
                }
            }
            //print server option
            /*try
            {
                LocalPrintServer localPrintServer = new LocalPrintServer();
                localPrintServer.InstallPrintQueue(printoptions["name"], printoptions["drivername"], new string[] { printoptions["portname"] }, "WinPrint", PrintQueueAttributes.None, "", "", "", "",  1, 1);
            }*/
            catch (Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");
                return e.ToString();
            }

            //return null;
        }
        public string Update(Dictionary<string, dynamic> queueoptions)
        {
            try
            {
                //ManagementObject printqueue = new ManagementObject(ms.Get(), new ManagementPath(@"win32_printer.DeviceId='" + queueoptions["name"] + "'"), new ObjectGetOptions());
                //var printqueue = new ManagementClass(ms.Get(), new ManagementObject(path), new ObjectGetOptions());
                //printqueue.Get();
                ManagementObject printqueue = Get(queueoptions["name"]);
                if (printqueue != null) {
                    //PrintQueue printqueue = new PrintQueue(localprintserver, queueoptions["name"], PrintSystemDesiredAccess.AdministratePrinter);
                    //printqueue.GetType().GetProperty("QueueDriver").SetValue(printqueue, "QueueDriver");
                    IDictionary<string, object> qo = queueoptions["options"];
                    var props = qo.Keys;
                    /*Console.WriteLine("Requested printer properties:");
                    foreach (var p in props)
                    {
                        Console.WriteLine(p + ": " + qo[p]);
                    }*/
                    foreach (var p in props)
                    {
                        if (queueprops.ContainsKey(p))
                        {
                            /*if(queueprops[p]["name"] == "QueuePort")
                            {
                                PrintDriver
                                printqueue.GetType().GetProperty(queueprops[p]["name"]).SetValue(printqueue, qo[p]);
                            } else
                            {
                                printqueue.GetType().GetProperty(queueprops[p]["name"]).SetValue(printqueue, qo[p]);
                            }*/
                            printqueue.SetPropertyValue(queueprops[p]["name"], qo[p]);
                        }
                    }

                    var npp = printqueue.Properties;
                    /*foreach (var p in npp)
                    {
                        Console.WriteLine(p.Name + ": " + printqueue.GetPropertyValue(p.Name));
                    }*/

                    PutOptions options = new PutOptions();
                    options.Type = PutType.UpdateOnly;
                    printqueue.Put(options);

                    updateCache(queueoptions["name"]);

                    return null;
                } else
                {
                    return "Failed to find a queue named " + queueoptions["name"];
                }
            }
            catch (Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");

                return e.ToString();
            }
            //return null;
        }
    }
}
