using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

//https://stackoverflow.com/questions/16040148/add-printer-to-local-computer-using-managementclass

namespace PrintManagement.pslib
{
    class addPort
    {
        static managementScope ms = managementScope.Instance;
        public string Create(string name, int protocol, string address, int port, bool snmp)
        {
            /*if (CheckPrinterPort())
                return true;*/
            try
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
            }
            catch(Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");
                return e.ToString();
            }

            return null;
        }
    }
}
