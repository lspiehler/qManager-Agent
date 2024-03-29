﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.WebSockets;
using Newtonsoft.Json;
using System.Threading;
using System.Net.NetworkInformation;

namespace PrintManagement.responderlib
{
    class ActionResponse
    {
        private pslib.printQueue printqueue = new pslib.printQueue();
        private pslib.printPort printport = new pslib.printPort();
        private pslib.printDriver printdriver = new pslib.printDriver();
        private wslib.responder wsresponser = new wslib.responder();
        static configHandler confighandler = configHandler.Instance;
        static Dictionary<string, string> config = confighandler.getConfig();
        private static string GetLocalhostFqdn()
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            return string.IsNullOrWhiteSpace(ipProperties.DomainName) ? ipProperties.HostName : string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName);
        }

        public static bool PropertyExists(dynamic obj, string name)
        {
            if (obj == null) return false;
            if (obj is IDictionary<string, object> dict)
            {
                return dict.ContainsKey(name);
            }
            return obj.GetType().GetProperty(name) != null;
        }
        public async Task ProcessResponse(System.Net.WebSockets.Managed.ClientWebSocket ws, dynamic rm)
        {
            string path = rm.body.path;
            HashTableResponseBody body = new HashTableResponseBody();

            if (path == "/printer/port/create")
            {
                try
                {
                    string result = printport.Create(rm.body.options.ip, 1, rm.body.options.ip, 9100, false);
                    if (result == null)
                    {
                        body.result = "success";
                        body.message = "The port was created successfully";
                    }
                    else
                    {
                        body.result = "error";
                        body.message = result;
                    }
                    body.data = null;
                    /*body.data = new Hashtable()
                    {
                        {"name", rm.body.options.name },
                        {"rowId", rm.body.options.rowId },
                        {"server", rm.body.options.server }
                    };*/
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }
            /*else if (path == "/printer/port/list")
            {
                try
                {
                    bool updatecache = false;
                    if (PropertyExists(rm.body.options, "updatecache"))
                    {
                        updatecache = rm.body.options.updatecache;
                    }
                    Dictionary<string, printerlib.GetPrinterPort> allports = printport.GetAll(updatecache);
                    if (allports == null)
                    {
                        body.result = "success";
                        body.message = null;
                    }
                    else
                    {
                        body.result = "error";
                        body.message = "Failed retrieving the list of ports";
                    }
                    body.result = "success";
                    body.message = null;
                    body.data = new Hashtable(allports);
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }*/
            else if (path == "/printer/queue/create")
            {
                try
                {
                    string result = printqueue.Create(new Dictionary<string, dynamic>(rm.body.options));
                    if (result == null)
                    {
                        body.result = "success";
                        body.message = "The print queue was created successfully";
                    }
                    else
                    {
                        body.result = "error";
                        body.message = result;
                    }
                    body.data = new Hashtable(new Dictionary<string, object>(rm.body.options));
                    /*body.data = new Hashtable()
                    {
                        {"name", rm.body.options.name },
                        {"rowId", rm.body.options.rowId },
                        {"server", rm.body.options.server }
                    };*/
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }
            else if (path == "/printer/queue/delete")
            {
                try
                {
                    string result = printqueue.Delete(rm.body.options.name);
                    if (result == null)
                    {
                        body.result = "success";
                        body.message = "The print queue was deleted successfully";
                    }
                    else
                    {
                        body.result = "error";
                        body.message = result;
                    }
                    body.data = new Hashtable()
                    {
                        {"name", rm.body.options.name },
                        {"rowId", rm.body.options.rowId },
                        {"server", rm.body.options.server }
                    };
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }
            else if (path == "/printer/queue/flush")
            {
                try
                {
                    string result = printqueue.Clear(rm.body.options.name);
                    if (result == null)
                    {
                        body.result = "success";
                        body.message = "Print jobs deleted successfully";
                    }
                    else
                    {
                        body.result = "error";
                        body.message = result;
                    }
                    body.data = new Hashtable()
                    {
                        {"name", rm.body.options.name },
                        {"rowId", rm.body.options.rowId },
                        {"server", rm.body.options.server }
                    };
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }
            else if (path == "/printer/queue/getconfig")
            {
                try
                {
                    Dictionary<string, string> result = printqueue.GetPrintSettings(rm.body.options);
                    if (result != null)
                    {
                        body.result = "success";
                        body.message = null;
                        body.data = new Hashtable(result);
                    }
                    else
                    {
                        body.result = "error";
                        body.message = "Failed to get print queue settings";
                        body.data = null;
                    }
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }
            /*else if (path == "/printer/queue/list")
            {
                try
                {
                    bool updatecache = false;
                    if (PropertyExists(rm.body.options, "updatecache"))
                    {
                        updatecache = rm.body.options.updatecache;
                    }
                    Dictionary<string, printerlib.GetPrinter> allprinters = printqueue.GetAll(updatecache);
                    if (allprinters == null)
                    {
                        body.result = "success";
                        body.message = null;
                    }
                    else
                    {
                        body.result = "error";
                        body.message = "Failed retrieving the list of printers";
                    }
                    body.result = "success";
                    body.message = null;
                    body.data = new Hashtable(allprinters);
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }*/
            else if (path == "/printer/queue/set")
            {
                try
                {
                    string result = printqueue.Update(new Dictionary<string, dynamic>(rm.body.options));
                    if (result == null)
                    {
                        body.result = "success";
                        body.message = "Queue options successfully updated";
                    }
                    else
                    {
                        body.result = "error";
                        body.message = result;
                    }
                    body.data = new Hashtable()
                    {
                        {"name", rm.body.options.name },
                        {"rowId", rm.body.options.rowId },
                        {"options", rm.body.options.options },
                        {"server", rm.body.options.server }
                    };
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }
            else if (path == "/printer/queue/setconfig")
            {
                try
                {
                    string result = printqueue.SetPrintSettings(rm.body.options.name, new Dictionary<string, dynamic>(rm.body.options));
                    if (result == null)
                    {
                        body.result = "success";
                        body.message = "Print settings updated successfully";
                    }
                    else
                    {
                        body.result = "error";
                        body.message = result;
                    }
                    body.data = new Hashtable()
                    {
                        {"name", rm.body.options.name },
                        {"rowId", rm.body.options.rowId },
                        {"options", rm.body.options.options },
                        {"server", rm.body.options.server }
                    };
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }
            else if (path == "/printer/queue/testpage")
            {
                try
                {
                    string result = printqueue.PrintTestPage(rm.body.options.name);
                    if (result == null)
                    {
                        body.result = "success";
                        body.message = "Test page submitted successfully";
                    }
                    else
                    {
                        body.result = "error";
                        body.message = result;
                    }
                    body.data = new Hashtable()
                    {
                        {"name", rm.body.options.name },
                        {"rowId", rm.body.options.rowId },
                        {"server", rm.body.options.server }
                    };
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }
            /*else if (path == "/printer/driver/list")
            {
                try
                {
                    bool updatecache = false;
                    if (PropertyExists(rm.body.options, "updatecache"))
                    {
                        updatecache = rm.body.options.updatecache;
                    }
                    Dictionary<string, printerlib.GetPrinterDriver> alldrivers = printdriver.Get(updatecache);

                    //update driver cache in the background for next request
                    Task.Factory.StartNew(() =>
                    {
                        printdriver.Get(true);
                    });

                        if (alldrivers == null)
                    {
                        body.result = "success";
                        body.message = null;
                    }
                    else
                    {
                        body.result = "error";
                        body.message = "Failed retrieving the list of drivers";
                    }
                    body.result = "success";
                    body.message = null;
                    body.data = new Hashtable(alldrivers);
                }
                catch(Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }*/
            else if (path == "/register")
            {
                try
                {
                    string[] groups;
                    if(config["Groups"] != null)
                    {
                        groups = config["Groups"].Split(new string[] { "," }, StringSplitOptions.None);
                    } else
                    {
                        groups = new string[] { };
                    }
                    body.result = "success";
                    body.message = null;
                    body.data = new Hashtable()
                    {
                        {"hostname", GetLocalhostFqdn()},
                        {"agentVersion", "0.94" },
                        {"groups", groups}
                    };
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }
            else
            {
                body.result = "error";
                body.message = "Unrecognized request to " + path;
                body.data = null;
            }

            //Console.WriteLine("and here");

            HashTableResponse resp = new HashTableResponse();

            resp.id = rm.id;
            resp.type = "response";
            resp.body = body;

            string jsonresp = JsonConvert.SerializeObject(resp);

            ArraySegment<byte> bytestosend = new ArraySegment<byte>(
                Encoding.UTF8.GetBytes(jsonresp)
            );

            //Console.WriteLine(jsonresp);

            try
            {
                //Console.WriteLine(DateTime.Now.ToString() + " - Begin");
                /*await ws.SendAsync(
                    bytestosend,
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );*/
                await wsresponser.Send(ws, bytestosend);
                //Console.WriteLine(DateTime.Now.ToString() + " - End");

                //ws.Dispose();
            }
            catch (Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");
            }
        }
    }
}
