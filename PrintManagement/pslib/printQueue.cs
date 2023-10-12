using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Printing;
//using System.Management.Automation;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PrintManagement.pslib
{
    class printQueue
    {
        static string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);

        [DllImport("printui.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern void PrintUIEntryW(IntPtr hwnd, IntPtr hinst, string lpszCmdLine, int nCmdShow);

        static managementScope ms = managementScope.Instance;
        static configHandler confighandler = configHandler.Instance;
        static Dictionary<string, string> config = confighandler.getConfig();
        static Dictionary<string, printerlib.GetPrinter> cachedobjects;
        //getPrinterPort getprinterport = new getPrinterPort();
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
                    { "name", "ShareName" }
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
            },
            { "Shared", new Dictionary<string, string>
                {
                    { "type", "bool" },
                    { "name", "Shared" }
                }
            },
            { "ShareName", new Dictionary<string, string>
                {
                    { "type", "string" },
                    { "name", "ShareName" }
                }
            }
        };

        public string PrintTestPage(string name)
        {
            Console.WriteLine(DateTime.Now.ToString() + " Printing test page to " + name);
            try
            {
                var printParams = string.Format(@"/k /q /n{0}", "\"" + name + "\"");
                PrintUIEntryW(IntPtr.Zero, IntPtr.Zero, printParams, 0);
                updateCache("TestPage", name);
            }
            catch (Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");
                return e.ToString();
            }

            return null;
        }

        private string updateCache(string action, string queuename)
        { // need to modify code for updating vs creating. Check if key exists in cache????
            //Console.WriteLine("Updating cache for queue name " + queuename);
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

                        if (cachedobjects != null)
                        {
                            if (cachedobjects.ContainsKey(queuename))
                            {
                                cachedobjects[queuename] = getobject;
                            }
                            else
                            {
                                cachedobjects.Add(queuename, getobject);
                            }
                        }

                        //only do this if script arg is set
                        if (config["Script"] != null)
                        {
                            if(File.Exists(config["Script"])) {
                                if (action == "Create" || action == "Update")
                                {
                                    printerlib.PSPrinterResult psprinterresult = new printerlib.PSPrinterResult();
                                    psprinterresult.Action = action;
                                    psprinterresult.Type = "Printer";
                                    psprinterresult.Printer = getobject;
                                    string jsonresult = JsonConvert.SerializeObject(psprinterresult);
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
                                        } else
                                        {
                                            return null;
                                        }
                                    }
                                } else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                return config["Script"] + " does not exist";
                            }
                        } else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        Console.WriteLine(DateTime.Now.ToString() + " Failed to find the port to update the cache");
                        return "Failed to find the port to update the cache";
                    }

                }
                else
                {
                    Console.WriteLine(DateTime.Now.ToString() + " Failed to find the printer to update the cache");
                    return "Failed to find the printer to update the cache";
                }
            }
            catch (Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");
                return e.ToString();
            }
        }

        public Dictionary<string, printerlib.GetPrinter> CIMGetAll(bool updatecache)
        {
            //https://stackoverflow.com/questions/978862/how-to-make-forward-only-read-only-wmi-queries-in-c
            if (updatecache == true || cachedobjects == null)
            {
                try
                {
                    Dictionary<string, printerlib.GetPrinter> objects = new Dictionary<string, printerlib.GetPrinter>();
                    Dictionary<string, printerlib.GetPrinterPort> printerports = printport.GetAll(updatecache);

                    //System.Management.ManagementObject oq = new System.Management.ObjectQuery("Select * from Win32_Printer");
                    EnumerationOptions options = new EnumerationOptions();
                    options.Rewindable = false;
                    options.ReturnImmediately = true;

                    SelectQuery query = new SelectQuery("Select * From CIM_Printer");
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
                Console.WriteLine(DateTime.Now.ToString() + " Returning cached printers");
                return cachedobjects;
            }
        }

        public Dictionary<string, printerlib.GetPrinter> GetAll(bool updatecache)
        {
            if (config["Legacy"] != null && config["Legacy"].ToLower() == "true")
            {
                return CIMGetAll(updatecache);
            }
            else
            {
                if (updatecache == true || cachedobjects == null)
                {
                    Console.WriteLine(DateTime.Now.ToString() + " Running powershell command");

                    //getPrinterPort getprinterport = new getPrinterPort();
                    Dictionary<string, printerlib.GetPrinterPort> printerports = printport.GetAll(updatecache);

                    Dictionary<string, printerlib.GetPrinter> objects = new Dictionary<string, printerlib.GetPrinter>();
                    try
                    {
                        using (System.Management.Automation.PowerShell PowerShellInst = System.Management.Automation.PowerShell.Create())
                        {
                            PowerShellInst.AddCommand("Get-Printer");

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
                                return null;
                            }

                            foreach (System.Management.Automation.PSObject obj in PSOutput)
                            {
                                printerlib.GetPrinter getobject = new printerlib.GetPrinter();
                                if (obj != null)
                                {
                                    List<System.Management.Automation.PSPropertyInfo> properties = obj.Properties.ToList();
                                    for (int i = 0; i < properties.Count; i++)
                                    {
                                        System.Management.Automation.PSPropertyInfo psobject = obj.Properties[properties[i].Name];
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
                    //Console.WriteLine("Used cached powershell command");
                    return cachedobjects;
                }
            }
        }
        public string Clear(string name)
        {
            try
            {
                PrintQueue printqueue = new PrintQueue(new LocalPrintServer(), name, PrintSystemDesiredAccess.AdministratePrinter);
                printqueue.Purge();
                updateCache("Clear", name);
            }
            catch (Exception e)
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
                    //Console.WriteLine(printqueue["Comment"]);
                    return printqueue;
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
                    if(cachedobjects != null) {
                        cachedobjects.Remove(name);
                    }
                    return null;
                }
                else
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
            /*try
            {
                ManagementObject printqueue = Get(printoptions["name"]);
                if (printqueue == null)
                {
                    //need to use this to create the printer and WMI to update because of access denied issues when creating printers with WMI on newer OSes
                    LocalPrintServer localPrintServer = new LocalPrintServer();
                    //PrintQueue printqueue = new PrintQueue(new LocalPrintServer(), name, PrintSystemDesiredAccess.AdministratePrinter);
                    localPrintServer.InstallPrintQueue(printoptions["name"], printoptions["drivername"], new string[] { printoptions["portname"] }, "WinPrint", PrintQueueAttributes.None);
                    //var printerClass = new ManagementClass(ms.Get(), new ManagementPath("Win32_Printer"), null);
                    //printerClass.Get();
                    //var printer = printerClass.CreateInstance();
                    //ManagementBaseObject printer = printerClass.GetMethodParameters("AddPrinterConnection");
                    var printerClass = new ManagementClass(ms.Get(), new ManagementPath("Win32_Printer"), new ObjectGetOptions());
                    printerClass.Get();
                    var printer = printerClass.CreateInstance();
                    var props = printoptions.Keys;
                    //Console.WriteLine("Requested printer properties:");
                    //foreach (var p in props)
                    //{
                    //    Console.WriteLine(p + ": " + printoptions[p]);
                    //}
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
                            if (p != "server" && p != "config")
                            {
                                Console.WriteLine("Failed to set unknown property " + p + " while creating print queue " + printoptions["name"]);
                            }
                        }
                    }
                    //printer.SetPropertyValue("Network", true);
                    //printer.SetPropertyValue("Default", false);
                    //printer.SetPropertyValue("DeviceID", printoptions["name"]);
                    if (printoptions.ContainsKey("sharename") == false)
                    {
                        printer.SetPropertyValue("ShareName", printoptions["name"]);
                    }
                    //printer.SetPropertyValue("WorkOffline", true);

                    //var npp = printer.Properties;
                    //foreach (var p in npp)
                    //{
                    //    Console.WriteLine(p.Name + ": " + printer.GetPropertyValue(p.Name));
                    //}

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
                    options.Type = PutType.UpdateOnly;
                    //options.Type = PutType.CreateOnly;
                    printer.Put(options);

                    updateCache(printoptions["name"]);

                    if(printoptions.ContainsKey("config"))
                    {
                        //Console.WriteLine(printoptions["name"].ToString());
                        //Console.WriteLine(JsonConvert.SerializeObject(printoptions["config"]));
                        //Console.WriteLine(JsonConvert.SerializeObject(printoptions["config"][0]));
                        string sps = SetPrintSettings(printoptions["name"].ToString(), new Dictionary<string, dynamic>(printoptions["config"][0]));
                        if(sps != null)
                        {
                            return "The print queue was created successfully, but applying print settings failed";
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return "A printer named " + printoptions["name"] + " already exists";
                }
            }*/
            //print server option
            try
            {
                ManagementObject printqueue = Get(printoptions["name"]);
                if (printqueue == null)
                {
                    List<PrintQueueAttributes> pqa = new List<PrintQueueAttributes>();
                    string sharename = "";
                    string comment = "";
                    string location = "";
                    pqa.Add(PrintQueueAttributes.None);
                    if (printoptions.ContainsKey("published") && printoptions["published"] == true)
                    {
                        pqa.Add(PrintQueueAttributes.Published);
                    }
                    if (printoptions.ContainsKey("shared") && printoptions["shared"] == true)
                    {
                        pqa.Add(PrintQueueAttributes.Shared);
                        if (printoptions.ContainsKey("sharename"))
                        {
                            sharename = printoptions["sharename"];
                        }
                        else
                        {
                            sharename = printoptions["name"];
                        }
                    }
                    if (printoptions.ContainsKey("comment"))
                    {
                        comment = printoptions["comment"];
                    }
                    if (printoptions.ContainsKey("location"))
                    {
                        location = printoptions["location"];
                    }
                    LocalPrintServer localPrintServer = new LocalPrintServer();
                    //PrintQueue printqueue = new PrintQueue(new LocalPrintServer(), name, PrintSystemDesiredAccess.AdministratePrinter);
                    localPrintServer.InstallPrintQueue(printoptions["name"], printoptions["drivername"], new string[] { printoptions["portname"] }, "WinPrint", (PrintQueueAttributes)Enum.Parse(typeof(PrintQueueAttributes), String.Join(",", pqa)), sharename, comment, location, null, 1, 0);

                    string ucresult = updateCache("Create", printoptions["name"]);

                    if (ucresult != null)
                    {
                        return ucresult;
                    }
                    else
                    {
                        if (printoptions.ContainsKey("config"))
                        {
                            //Console.WriteLine(printoptions["name"].ToString());
                            //Console.WriteLine(JsonConvert.SerializeObject(printoptions["config"]));
                            //Console.WriteLine(JsonConvert.SerializeObject(printoptions["config"][0]));
                            string sps = SetPrintSettings(printoptions["name"].ToString(), new Dictionary<string, dynamic>(printoptions["config"][0]));
                            if (sps != null)
                            {
                                return "The print queue was created successfully, but applying print settings failed";
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    return "A printer named " + printoptions["name"] + " already exists";
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
        public string Update(Dictionary<string, dynamic> queueoptions)
        {
            try
            {
                //ManagementObject printqueue = new ManagementObject(ms.Get(), new ManagementPath(@"win32_printer.DeviceId='" + queueoptions["name"] + "'"), new ObjectGetOptions());
                //var printqueue = new ManagementClass(ms.Get(), new ManagementObject(path), new ObjectGetOptions());
                //printqueue.Get();
                ManagementObject printqueue = Get(queueoptions["name"]);
                if (printqueue != null)
                {
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

                    if(qo.ContainsKey("Shared"))
                    {
                        if (qo["Shared"].ToString() == "True" || (printqueue["Shared"].ToString() == "True" && qo["Shared"].ToString() == "False"))
                        {
                            if (qo.ContainsKey("ShareName"))
                            {
                                if (qo["ShareName"] == null || qo["ShareName"].ToString() == "")
                                {
                                    return "ShareName cannot be blank on a shared queue";
                                }
                            } else
                            {
                                if (printqueue["ShareName"] == null || printqueue["ShareName"].ToString() == "")
                                {
                                    printqueue.SetPropertyValue(queueprops["ShareName"]["name"], queueoptions["name"]);
                                }
                            }
                        }
                    } else
                    {
                        if(printqueue["Shared"].ToString() == "True")
                        {
                            if (qo.ContainsKey("ShareName"))
                            {
                                if (qo["ShareName"] == null || qo["ShareName"].ToString() == "")
                                {
                                    return "ShareName cannot be blank on a shared queue";
                                }
                            }
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

                    string ucresult = updateCache("Update", queueoptions["name"]);

                    if (ucresult != null)
                    {
                        return ucresult;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
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
        public Dictionary<string, string> GetPrintSettings(dynamic options)
        {
            //Console.WriteLine(options.name);
            Dictionary<string, string> pp = new Dictionary<string, string>();
            string type;
            if (options.GetType().GetProperty("type") == null)
            {
                type = "8";
            }
            else
            {
                type = options.type;
            }
            try
            {
                //var stdOutBuffer = new StringBuilder();
                //var stdErrBuffer = new StringBuilder();
                Process p = new Process();
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                Console.WriteLine(DateTime.Now.ToString() + " " + strWorkPath + "\\setprinter.exe");
                p.StartInfo.FileName = strWorkPath + "\\setprinter.exe";
                //Console.WriteLine("-show \"" + options.name + "\" " + type);
                p.StartInfo.Arguments = "-show \"" + options.name + "\" " + type;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                /*string output;
                string error;

                using (System.IO.StreamReader myOutput = p.StandardOutput)
                {
                    output = myOutput.ReadToEnd();
                }
                using (System.IO.StreamReader myError = p.StandardError)
                {
                    error = myError.ReadToEnd();

                }*/
                string sResult = p.StandardOutput.ReadToEnd();
                //string eResult = p.StandardError.ReadToEnd();

                //Console.WriteLine("this?");
                string[] lines = sResult.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                ////List<string> trimmedlines = new List<string>();

                for (int i = 2; i < lines.Count() - 2; i++)
                {
                    //Console.WriteLine(lines[i]);
                    string[] kv = lines[i].Trim().Split(new string[] { "=" }, StringSplitOptions.None);
                    //Console.WriteLine(kv[0].Trim() + ": " + kv[1].Trim().Replace("\"", ""));
                    //Console.WriteLine(kv[1]);
                    ////trimmedlines.Add(lines[i].Trim());
                    pp.Add(kv[0], kv[1].Replace("\"", ""));
                }

                /*trimmedlines.Sort();

                for (int i = 2; i < trimmedlines.Count - 2; i++)
                {
                    Console.WriteLine(trimmedlines[i]);
                    string[] kv = trimmedlines[i].Split(new string[] { "=" }, StringSplitOptions.None);
                    pp.Add(kv[0], kv[1].Replace("\"", ""));
                }*/

                    //Console.WriteLine(error);

                    return pp;
                /*var cmd = Cli.Wrap("setprinter.exe");
                cmd.SetArguments(new string[] { options.name, type });
                //cmd.st
                Console.WriteLine("\"" + options.name + "\" " + type);
                cmd.re
                var result = cmd.Execute();
                Console.WriteLine(result.StandardOutput);
                Console.WriteLine(result.StandardError);
                return result.StandardOutput;*/
            }
            catch (Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");
                return pp;
            }
        }
        public string SetPrintSettings(string name, Dictionary<string, dynamic> printoptions)
        {
            //Console.WriteLine("Debugging here: ");
            //Console.WriteLine(JsonConvert.SerializeObject(name));
            ManagementObject printqueue = Get(name);
            if (printqueue != null)
            {
                try
                {
                    IDictionary<string, object> po = printoptions["options"];
                    string type;
                    if (printoptions.ContainsKey("type"))
                    {
                        type = printoptions["type"].ToString();
                    }
                    else
                    {
                        type = "8";
                    }
                    //PrintQueue printqueue = new PrintQueue(localprintserver, queueoptions["name"], PrintSystemDesiredAccess.AdministratePrinter);
                    //printqueue.GetType().GetProperty("QueueDriver").SetValue(printqueue, "QueueDriver");
                    var props = po.Keys;
                    /*Console.WriteLine("Requested printer properties:");
                    foreach (var p in props)
                    {
                        Console.WriteLine(p + ": " + qo[p]);
                    }*/
                    List<string> optionargs = new List<string>();
                    foreach (var prop in props)
                    {
                        //Console.WriteLine(prop + "=" + po[prop]);
                        optionargs.Add(prop + "=" + po[prop]);
                    }

                    Process p = new Process();
                    // Redirect the output stream of the child process.
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.FileName = "setprinter.exe";
                    //Console.WriteLine("\"" + name + "\" " + type + " pDevMode=" + String.Join(",", optionargs));
                    p.StartInfo.Arguments = "\"" + name + "\" " + type + " pDevMode=" + String.Join(",", optionargs);
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();

                    //string sResult = p.StandardOutput.ReadToEnd();
                    string eResult = p.StandardError.ReadToEnd();

                    /*Console.WriteLine("Stdout:");
                    Console.WriteLine(sResult);
                    Console.WriteLine("Stderr:");
                    Console.WriteLine(eResult);*/
                    if(String.IsNullOrEmpty(eResult))
                    {
                        return null;
                    }
                    else
                    {
                        return eResult;
                    }
                }
                catch(Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                    return e.ToString();
                }
            }
            else
            {
                return "Failed to find a queue named " + name;
            }
        }
    }
}