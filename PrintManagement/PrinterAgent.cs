using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Printing;
using Newtonsoft.Json;

namespace PrintManagement
{
    public class PrinterAgent
    {
        public PrintQueueCollection getPrintQueues()
        {
            EnumeratedPrintQueueTypes[] types = new EnumeratedPrintQueueTypes[] {
                    EnumeratedPrintQueueTypes.Local,
                    EnumeratedPrintQueueTypes.Connections
                };

            PrintQueueIndexedProperty[] props = new PrintQueueIndexedProperty[] {
                    PrintQueueIndexedProperty.Name,
                    PrintQueueIndexedProperty.QueueAttributes
                };

            LocalPrintServer localPrintServer = new LocalPrintServer();
            PrintQueueCollection myPrintQueues = localPrintServer.GetPrintQueues(props, types);

            return myPrintQueues;
        }
    }
}
