using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Printing;
using Newtonsoft.Json;
using System.Collections;

namespace PrintManagement
{
    public class PMPrintQueues
    {

        private Hashtable cachedqueues = new Hashtable();
        private class PMPrintQueue
        {
            public bool InPartialTrust;
            //public int ClientPrintSchemaVersion;
            //public int MaxPrintSchemaVersion;
            public bool IsXpsDevice;
            public bool IsPublished;
            public bool IsRawOnlyEnabled;
            public bool IsBidiEnabled;
            public bool ScheduleCompletedJobsFirst;
            public bool KeepPrintedJobs;
            public bool IsDevQueryEnabled;
            public bool IsHidden;
            public bool Shared;
            public bool IsDirect;
            public bool IsQueued;
            public bool IsPowerSaveOn;
            public bool IsServerUnknown;
            public bool IsDoorOpened;
            public bool IsOutOfMemory;
            public bool NeedUserIntervention;
            public bool PagePunt;
            public bool HasToner;
            public bool IsTonerLow;
            public bool IsWarmingUp;
            public bool IsInitializing;
            public bool IsProcessing;
            public bool IsWaiting;
            public bool IsNotAvailable;
            public bool IsOutputBinFull;
            public bool IsPrinting;
            public bool IsBusy;
            public bool IsIOActive;
            public bool IsOffline;
            public bool HasPaperProblem;
            public bool IsManualFeedRequired;
            public bool IsOutOfPaper;
            public bool IsPaperJammed;
            public bool IsPendingDeletion;
            public bool IsInError;
            public bool IsPaused;
            public int QueueAttributes;
            public string QueueStatus;
            public string FullName;
            public string HostingPrintServer;
            public string QueuePrintProcessor;
            public string PortName;
            public string PrinterHostAddress;
            public string DriverName;
            //public string DefaultPrintTicket;
            //public string UserPrintTicket;
            public string SeparatorFile;
            public string Description;
            public string Location;
            public string Comment;
            public string ShareName;
            public int NumberOfJobs;
            public int AveragePagesPerMinute;
            public int UntilTimeOfDay;
            public int StartTimeOfDay;
            public int DefaultPriority;
            public string Name;
            public int Priority;
            //public string CurrentJobSettings;
            public bool PrintingIsCancelled;
            //public string Parent;
            //public string PropertiesCollection;

        }
        public Hashtable getQueues()
        {
            if (cachedqueues.Count > 0)
            {
                Console.WriteLine("returning cached queues");
                return cachedqueues;
            }
            else
            {
                EnumeratedPrintQueueTypes[] types = new EnumeratedPrintQueueTypes[] {
                        EnumeratedPrintQueueTypes.Local,
                        EnumeratedPrintQueueTypes.Connections
                    };

                PrintQueueIndexedProperty[] props = new PrintQueueIndexedProperty[] {
                        PrintQueueIndexedProperty.Name,
                        PrintQueueIndexedProperty.QueueAttributes,
                        PrintQueueIndexedProperty.QueueStatus
                    };

                LocalPrintServer localPrintServer = new LocalPrintServer();
                PrintQueueCollection myPrintQueues = localPrintServer.GetPrintQueues(props, types);

                cachedqueues = FormatResponse(myPrintQueues);
                return cachedqueues;
            }
        }
        private Hashtable FormatResponse(PrintQueueCollection pqc)
        {
            //try
            //{
            var props = pqc.First().GetType().GetProperties();
            foreach (var p in props)
            {
                Console.WriteLine(p.Name);
            }

            Console.WriteLine();

            //List<PMPrintQueue> queues = new List<PMPrintQueue>();

            Hashtable queues = new Hashtable();

            foreach (PrintQueue pq in pqc)
                {
                    Console.WriteLine(pq.Name);
                    PMPrintQueue pqo = new PMPrintQueue();
                    pqo.InPartialTrust = pq.InPartialTrust;
                    //pqo.ClientPrintSchemaVersion = pq.ClientPrintSchemaVersion;
                    //MaxPrintSchemaVersion = pq.MaxPrintSchemaVersion;
                    pqo.IsXpsDevice = pq.IsXpsDevice;
                    pqo.IsPublished = pq.IsPublished;
                    pqo.IsRawOnlyEnabled = pq.IsRawOnlyEnabled;
                    pqo.IsBidiEnabled = pq.IsBidiEnabled;
                    pqo.ScheduleCompletedJobsFirst = pq.ScheduleCompletedJobsFirst;
                    pqo.KeepPrintedJobs = pq.KeepPrintedJobs;
                    pqo.IsDevQueryEnabled = pq.IsDevQueryEnabled;
                    pqo.IsHidden = pq.IsHidden;
                    pqo.Shared = pq.IsShared;
                    pqo.IsDirect = pq.IsDirect;
                    pqo.IsQueued = pq.IsQueued;
                    pqo.IsPowerSaveOn = pq.IsPowerSaveOn;
                    pqo.IsServerUnknown = pq.IsServerUnknown;
                    pqo.IsDoorOpened = pq.IsDoorOpened;
                    pqo.IsOutOfMemory = pq.IsOutOfMemory;
                    pqo.NeedUserIntervention = pq.NeedUserIntervention;
                    pqo.PagePunt = pq.PagePunt;
                    pqo.HasToner = pq.HasToner;
                    pqo.IsTonerLow = pq.IsTonerLow;
                    pqo.IsWarmingUp = pq.IsWarmingUp;
                    pqo.IsInitializing = pq.IsInitializing;
                    pqo.IsProcessing = pq.IsProcessing;
                    pqo.IsWaiting = pq.IsWaiting;
                    pqo.IsNotAvailable = pq.IsNotAvailable;
                    pqo.IsOutputBinFull = pq.IsOutputBinFull;
                    pqo.IsPrinting = pq.IsPrinting;
                    pqo.IsBusy = pq.IsBusy;
                    pqo.IsIOActive = pq.IsIOActive;
                    pqo.IsOffline = pq.IsOffline;
                    pqo.HasPaperProblem = pq.HasPaperProblem;
                    pqo.IsManualFeedRequired = pq.IsManualFeedRequired;
                    pqo.IsOutOfPaper = pq.IsOutOfPaper;
                    pqo.IsPaperJammed = pq.IsPaperJammed;
                    pqo.IsPendingDeletion = pq.IsPendingDeletion;
                    pqo.IsInError = pq.IsInError;
                    pqo.IsPaused = pq.IsPaused;
                    pqo.QueueAttributes = (int)pq.QueueAttributes;
                    pqo.QueueStatus = pq.QueueStatus.ToString();
                    pqo.FullName = pq.FullName;
                    pqo.HostingPrintServer = pq.HostingPrintServer.Name;
                    pqo.QueuePrintProcessor = pq.QueuePrintProcessor.Name;
                    pqo.PortName = pq.QueuePort.Name;
                    pqo.PrinterHostAddress = pq.QueuePort.Name;
                    pqo.DriverName = pq.QueueDriver.Name;
                    //DefaultPrintTicket = pq.DefaultPrintTicket;
                    //UserPrintTicket = pq.UserPrintTicket;
                    pqo.SeparatorFile = pq.SeparatorFile;
                    pqo.Description = pq.Description;
                    pqo.Location = pq.Location;
                    pqo.Comment = pq.Comment;
                    pqo.ShareName = pq.ShareName;
                    pqo.NumberOfJobs = pq.NumberOfJobs;
                    pqo.AveragePagesPerMinute = pq.AveragePagesPerMinute;
                    pqo.UntilTimeOfDay = pq.UntilTimeOfDay;
                    pqo.StartTimeOfDay = pq.StartTimeOfDay;
                    pqo.DefaultPriority = pq.DefaultPriority;
                    pqo.Name = pq.Name;
                    pqo.Priority = pq.Priority;
                    //pqo.CurrentJobSettings = pq.CurrentJobSettings.ToString();
                    pqo.PrintingIsCancelled = pq.PrintingIsCancelled;
                    //pqo.Parent = pq.Parent.ToString() ?? null;
                    //pqo.PropertiesCollection = pq.PropertiesCollection.ToString();

                    queues.Add(pq.Name, pqo);
                }

            //PMQueueResponse resp = new PMQueueResponse();

            //resp.result = "success";
            //resp.message = null;
            //resp.data = queues;

                return queues;
            /*}
            catch (Exception e)
            {
                Console.WriteLine("{0}Implicitly specified:{0}{1}",
               Environment.NewLine, e.StackTrace);

                return new List<PMPrintQueue>();
            }*/
        }
    }
}
