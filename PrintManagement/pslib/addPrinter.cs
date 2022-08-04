using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
//using System.Printing;

namespace PrintManagement.pslib
{
    class addPrinter
    {
        static managementScope ms = managementScope.Instance;
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

        //https://www.codeproject.com/Articles/80680/Managing-Printers-Programatically-using-C-and-WMI
        //https://www.developerfusion.com/article/5450/using-wmi-from-managed-code/2/
        public string Create(Dictionary<string, dynamic> printoptions)
        {
            Console.WriteLine("function called");
            /*if (CheckPrinterPort())
                return true;*/

            //wmi method
            try
            {
                //var printerClass = new ManagementClass(ms.Get(), new ManagementPath("Win32_Printer"), null);
                //printerClass.Get();
                //var printer = printerClass.CreateInstance();
                //ManagementBaseObject printer = printerClass.GetMethodParameters("AddPrinterConnection");
                var printerClass = new ManagementClass(ms.Get(), new ManagementPath("Win32_Printer"), new ObjectGetOptions());
                printerClass.Get();
                var printer = printerClass.CreateInstance();
                var props = printoptions.Keys;
                Console.WriteLine("Requested printer properties:");
                foreach (var p in props)
                {
                    Console.WriteLine(p + ": " + printoptions[p]);
                }
                foreach (var p in props)
                {
                    //Console.WriteLine(p);
                    //Console.WriteLine(printoptions[p]);
                    if(printprops.ContainsKey(p))
                    {
                        if(printprops[p]["type"] == "bool")
                        {
                            printer.SetPropertyValue(printprops[p]["name"], printoptions[p]);
                            Console.WriteLine(printprops[p]["name"] + " = " + printoptions[p] + " - " + printoptions[p].GetType().ToString());
                        }
                        else if(printprops[p]["type"] == "int")
                        {
                            printer.SetPropertyValue(printprops[p]["name"], printoptions[p]);
                            Console.WriteLine(printprops[p]["name"] + " = " + printoptions[p] + " - " + printoptions[p].GetType().ToString());
                        }
                        else
                        {
                            printer.SetPropertyValue(printprops[p]["name"], printoptions[p]);
                            Console.WriteLine(printprops[p]["name"] + " = " + printoptions[p] + " - " + printoptions[p].GetType().ToString());
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
                if(printoptions.ContainsKey("sharename") == false)
                {
                    printer.SetPropertyValue("ShareName", printoptions["name"]);
                }
                //printer.SetPropertyValue("WorkOffline", true);

                var npp = printer.Properties;
                foreach (var p in npp)
                {
                    Console.WriteLine(p.Name + ": " + printer.GetPropertyValue(p.Name));
                }

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
                options.Type = PutType.UpdateOrCreate;
                printer.Put(options);
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

            return null;
        }
    }
}
