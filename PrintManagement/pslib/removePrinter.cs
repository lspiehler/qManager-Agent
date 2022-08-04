using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Management;
using System.Printing;

namespace PrintManagement.pslib
{
    class removePrinter
    {
        public string Delete(string name)
        {
            Console.WriteLine("function called");
            /*if (CheckPrinterPort())
                return true;*/

            //wmi method
            try
            {
                //LocalPrintServer localPrintServer = new LocalPrintServer();
                //PrintQueue printqueue = new PrintQueue(localPrintServer, name, PrintSystemDesiredAccess.AdministratePrinter);
                bool result = LocalPrintServer.DeletePrintQueue(name);
                if(result)
                {
                    return null;
                }
                else
                {
                    return "Failed to delete the print queue";
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
