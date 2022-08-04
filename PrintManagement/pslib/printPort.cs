using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace PrintManagement.pslib
{
    class printPort
    {

        static managementScope ms = managementScope.Instance;

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
    }
}
