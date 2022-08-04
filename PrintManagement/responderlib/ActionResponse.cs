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
        //private pslib.addPrinter addprinter = new pslib.addPrinter();
        //private pslib.removePrinter removeprinter = new pslib.removePrinter();
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

            Console.WriteLine("and here");

            HashTableResponse resp = new HashTableResponse();

            resp.id = rm.id;
            resp.type = "response";
            resp.body = body;

            string jsonresp = JsonConvert.SerializeObject(resp);

            ArraySegment<byte> bytestosend = new ArraySegment<byte>(
                Encoding.UTF8.GetBytes(jsonresp)
            );

            Console.WriteLine(jsonresp);

            try
            {
                await ws.SendAsync(
                    bytestosend,
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );

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
