using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Printing;

namespace PrintManagement.pslib
{
    class printQueue
    {
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
    }
}
