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
    class getPrinterDriver
    {
        static configHandler confighandler = configHandler.Instance;
        static Dictionary<string, string> config = confighandler.getConfig();
        public class responseBody
        {
            public string result { get; set; }
            public string message { get; set; }
            public Dictionary<string, printerlib.GetPrinterDriver> data { get; set; }
        }

        Dictionary<string, printerlib.GetPrinterDriver> cachedobjects;
        public Dictionary<string, printerlib.GetPrinterDriver> Run(bool updatecache)
        {
            if (updatecache == true || cachedobjects == null)
            {
                Dictionary<string, printerlib.GetPrinterDriver> objects = new Dictionary<string, printerlib.GetPrinterDriver>();

                try
                {
                    using (PowerShell PowerShellInst = PowerShell.Create())
                    {
                        if (config["Legacy"] != null && config["Legacy"].ToLower() == "true")
                        {
                            Console.WriteLine("using legacy command");
                            // change this to the below and try converting to native C#?
                            //PowerShellInst.AddCommand("Get-CimInstance").AddParameter("ClassName", "Win32_PrinterDriver");
                            PowerShellInst.AddCommand("Get-WmiObject").AddParameter("Query", "SELECT * FROM Win32_PrinterDriver");
                        }
                        else
                        {
                            PowerShellInst.AddCommand("Get-PrinterDriver");
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
                            printerlib.GetPrinterDriver getobject = new printerlib.GetPrinterDriver();
                            if (obj != null)
                            {
                                List<PSPropertyInfo> properties = obj.Properties.ToList();
                                for (int i = 0; i < properties.Count; i++)
                                {
                                    PSPropertyInfo psobject = obj.Properties[properties[i].Name];
                                    //Console.WriteLine(properties[i].Name);
                                    //Console.WriteLine(psobject.GetType().ToString());
                                    if (psobject.GetType().ToString() == "System.Management.Automation.PSAdaptedProperty" || psobject.GetType().ToString() == "System.Management.Automation.PSProperty")
                                    {
                                        //don't include weird property names starting with "__"
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
                            if (config["Legacy"] != null && config["Legacy"].ToLower() == "true")
                            {
                                //get name on the left sife of the first comma
                                string printername = obj.Properties["Name"].Value.ToString();
                                string modifiedname = printername.Substring(0, printername.IndexOf(","));

                                // prevent returning duplicate names for 32 and 64-bit drivers
                                if (objects.ContainsKey(modifiedname) == false)
                                {
                                    objects.Add(modifiedname, getobject);
                                }
                            } else
                            {
                                string printername = obj.Properties["Name"].Value.ToString();
                                // prevent returning duplicate names for 32 and 64-bit drivers
                                if (objects.ContainsKey(printername) == false)
                                {
                                    objects.Add(printername, getobject);
                                }
                            }
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
