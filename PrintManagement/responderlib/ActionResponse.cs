using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.WebSockets;
using Newtonsoft.Json;
using System.Threading;

namespace PrintManagement.responderlib
{
    class ActionResponse
    {
        private pslib.testPage testpage = new pslib.testPage();
        private pslib.printQueue printqueue = new pslib.printQueue();
        private pslib.printPort printport = new pslib.printPort();
        //private pslib.queueProperties queueproperties = new pslib.queueProperties();
        private wslib.responder wsresponser = new wslib.responder();
        //private pslib.addPrinter addprinter = new pslib.addPrinter();
        //private pslib.addPrinter addprinter = new pslib.addPrinter();
        //private pslib.removePrinter removeprinter = new pslib.removePrinter();

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
                    string result = printport.Create(rm.body.options.ip, 1, rm.body.options.ip, 9100, true);
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
            else if (path == "/printer/port/list")
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
            }
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
            else if (path == "/printer/queue/list")
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
            }
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
                    string result = testpage.Print(rm.body.options.name);
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
            else if (path == "/printer/driver/list")
            {

            }
            else
            {

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
