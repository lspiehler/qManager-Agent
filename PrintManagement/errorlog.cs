using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement
{
    class errorlog
    {
        public void write(string message)
        {
            write(message, "", "information");
        }
        public void write(string message, string st, string type)
        {
            EventLogEntryType level;
            switch (type)
            {
                case "error":
                    // Write an 'Error' entry in specified log of event log.
                    level = EventLogEntryType.Error;
                    break;
                case "warning":
                    // Write a 'Warning' entry in specified log of event log.
                    level = EventLogEntryType.Warning;
                    break;
                case "information":
                    // Write an 'Information' entry in specified log of event log.
                    level = EventLogEntryType.Information;
                    break;
                /*case 4:
                    // Write a 'FailureAudit' entry in specified log of event log.
                    myEventLog.WriteEntry(myMessage, EventLogEntryType.FailureAudit, myID);
                    break;
                case 5:
                    // Write a 'SuccessAudit' entry in specified log of event log.
                    myEventLog.WriteEntry(myMessage, EventLogEntryType.SuccessAudit, myID);
                    break;*/
                default:
                    level = EventLogEntryType.Information;
                    break;
            }

            var appLog = new EventLog("Application");
            appLog.Source = "Print Management";
            appLog.WriteEntry(message + "\r\n" + st, level);
            if (type == "error")
            {
                Console.WriteLine(message + "\r\n" + st);

            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}
