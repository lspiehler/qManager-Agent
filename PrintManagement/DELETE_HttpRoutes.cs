using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Printing;
using Newtonsoft.Json;
using System.Collections;

namespace PrintManagement
{
    public class DELETE_HttpRoutes : Nancy.NancyModule
    {

        PMPrintQueues pmprintqueues = new PMPrintQueues();
        public DELETE_HttpRoutes()
        {
            Get["/printers"] = parameters =>
            {
                Console.WriteLine(Context.Request.Headers.ToString());
                PMPrintQueues pmprintqueues = new PMPrintQueues();

                Hashtable myPrintQueues = pmprintqueues.getQueues();

                return myPrintQueues;
            };

            Get["/"] = _ => "Received GET request";

            Post["/"] = _ => "Received POST request";

            Get["/"] = _ => "Let me have the request!";
        }
    }
}
