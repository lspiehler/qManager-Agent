using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Collections;
using Newtonsoft.Json;
using System.Threading;

namespace PrintManagement.responderlib
{
    class QueuedResponse
    {
        Dictionary<string, ResponderLibCache> cachedtypes = new Dictionary<string, ResponderLibCache>();
        pslib.printQueue printqueue = new pslib.printQueue();
        pslib.printDriver printdriver = new pslib.printDriver();
        private wslib.responder wsresponser = new wslib.responder();
        pslib.printPort printport = new pslib.printPort();

        //List<ClientWebSocket> clients = new List<ClientWebSocket>();
        //ArraySegment<byte> cachedresponse;

        public static bool PropertyExists(dynamic obj, string name)
        {
            if (obj == null) return false;
            if (obj is IDictionary<string, object> dict)
            {
                return dict.ContainsKey(name);
            }
            return obj.GetType().GetProperty(name) != null;
        }

        private class ResponderLibCache
        {
            //public string path { get; set; }
            public Dictionary<string, System.Net.WebSockets.Managed.ClientWebSocket> clients { get; set; }
            //public HashTableResponseBody cachedresponse { get; set; }
        }

        public async Task ProcessResponse(System.Net.WebSockets.Managed.ClientWebSocket ws, dynamic rm)
        {
            string path = rm.body.path;
            bool updatecache = false;
            if (PropertyExists(rm.body.options, "updatecache"))
            {
                updatecache = rm.body.options.updatecache;
            }
            //build cache object if it doesn't exist yet
            if (cachedtypes.ContainsKey(path) == false)
            {
                Console.WriteLine("Creating key " + path);
                ResponderLibCache rlc = new ResponderLibCache();
                //rlc.clients = new List<System.Net.WebSockets.Managed.ClientWebSocket>();
                //rlc.clients.Add(rm.id, ws);
                rlc.clients = new Dictionary<string, System.Net.WebSockets.Managed.ClientWebSocket>();
                //rlc.cachedresponse = null;
                cachedtypes.Add(path, rlc);
            }

            /*if (cachedtypes[path].cachedresponse != null && updatecache != true)
            {
                HashTableResponse resp = new HashTableResponse();

                resp.id = rm.id;
                resp.type = "response";
                resp.body = cachedtypes[path].cachedresponse;

                string jsonresp = JsonConvert.SerializeObject(resp);

                ArraySegment<byte> bytestosend = new ArraySegment<byte>(
                        Encoding.UTF8.GetBytes(jsonresp)
                    );

                Console.WriteLine("Sending cached response");
                try
                {
                    await wsresponser.Send(ws, bytestosend);
                }
                catch (Exception e)
                {
                    errorlog el = new errorlog();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }
            else
            {*/
            if (cachedtypes[path].clients.Count > 0)
            {
                // just add pending request to the queue
                Console.WriteLine("Adding pending request to the queue");
                cachedtypes[path].clients.Add(rm.id, ws);
            }
            else
            {
                cachedtypes[path].clients.Add(rm.id, ws);

                HashTableResponseBody body = new HashTableResponseBody();
                if (path == "/printer/queue/list")
                {
                    //PMPrintQueues pmprintqueues = new PMPrintQueues();

                    //Hashtable myPrintQueues = pmprintqueues.getQueues();
                    //pslib.getPrinter getprinter = new pslib.getPrinter();
                    try
                    {
                        body.result = "success";
                        body.message = null;
                        body.data = new Hashtable(printqueue.GetAll(updatecache));
                    }
                    catch(Exception e)
                    {
                        body.result = "error";
                        body.message = e.ToString();
                        body.data = null;

                        errorlog el = new errorlog();
                        el.write(e.ToString(), Environment.StackTrace, "error");
                    }
                }
                else if (path == "/printer/port/list")
                {
                    //pslib.getPrinterPort getprinterport = new pslib.getPrinterPort();
                    try
                    {
                        body.result = "success";
                        body.message = null;
                        body.data = new Hashtable(printport.GetAll(updatecache));
                    }
                    catch (Exception e)
                    {
                        body.result = "error";
                        body.message = e.ToString();
                        body.data = null;

                        errorlog el = new errorlog();
                        el.write(e.ToString(), Environment.StackTrace, "error");
                    }
                }
                else if (path == "/printer/driver/list")
                {
                    try
                    {
                        body.result = "success";
                        body.message = null;
                        body.data = new Hashtable(printdriver.Get(updatecache));
                    }
                    catch (Exception e)
                    {
                        body.result = "error";
                        body.message = e.ToString();
                        body.data = null;

                        errorlog el = new errorlog();
                        el.write(e.ToString(), Environment.StackTrace, "error");
                    }
                }
                else
                {

                }

                Console.WriteLine("Responding to all requests");

                List<string> clientkeys = new List<string>(cachedtypes[path].clients.Keys);

                for (int i = cachedtypes[path].clients.Count - 1; i >= 0; i--)
                {

                    HashTableResponse resp = new HashTableResponse();

                    //cachedtypes[path].cachedresponse = body;

                    resp.id = clientkeys[i];
                    resp.type = "response";
                    resp.body = body;

                    string jsonresp = JsonConvert.SerializeObject(resp, Formatting.Indented);

                    //Console.WriteLine(jsonresp);

                    ArraySegment<byte> bytestosend = new ArraySegment<byte>(
                        Encoding.UTF8.GetBytes(jsonresp)
                    );

                    //Console.WriteLine("Sending response to message id " + clientkeys[i]);
                    /*try
                    {
                        await cachedtypes[path].clients[clientkeys[i]].SendAsync(
                            bytestosend,
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                        );

                        //cachedtypes[path].clients[clientkeys[i]].Dispose();
                    }
                    catch(Exception e)
                    {
                        errorlog el = new errorlog();
                        el.write(e.ToString(), Environment.StackTrace, "error");
                    }*/
                    try
                    {
                        await wsresponser.Send(cachedtypes[path].clients[clientkeys[i]], bytestosend);
                    }
                    catch (Exception e)
                    {
                        errorlog el = new errorlog();
                        el.write(e.ToString(), Environment.StackTrace, "error");
                    }
                    cachedtypes[path].clients.Remove(clientkeys[i]);
                }

                Console.WriteLine(cachedtypes[path].clients.Count + " remaining");
            }
            //}
        }
    }
}
