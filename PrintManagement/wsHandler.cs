using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Dynamic;

namespace PrintManagement
{
    class wsHandler
    {
        //responderlib.CachedResponse cr = new responderlib.CachedResponse();
        responderlib.ActionResponse ar = new responderlib.ActionResponse();
        responderlib.QueuedResponse qr = new responderlib.QueuedResponse();
        //responderlib.RegisterResponse rr = new responderlib.RegisterResponse();

        /*Dictionary<string, Action<ClientWebSocket, dynamic>> routes = new Dictionary<string, Action<ClientWebSocket, dynamic>>();

        private wsHandler()
        {
            routes.Add("/listprinters", cr.ProcessResponse);
        }*/

        //public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

        /*public ArraySegment<byte> Compress(ReadOnlyMemory<byte> inputData)
        {
            var compressedStream = new MemoryStream();
            using (var gzipStream = new GZipStream(compressedStream, CompressionLevel, false))
            {
                gzipStream.Write(inputData.Span);
            }

            if (compressedStream.TryGetBuffer(out var buffer))
            { return buffer; }
            //else
            //{ return compressedStream.ToArray(); }
            //return compressedStream.GetBuffer();
        }*/

        /*public byte[] Compress(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
                {
                    gzipStream.Write(bytes, 0, bytes.Length);
                }
                //return memoryStream.ToArray();

                //memoryStream.TryGetBuffer(out var buffer);
                //return buffer;
                //else
                return memoryStream.ToArray();
            }
        }

        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        private static string GetLocalhostFqdn()
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            return string.IsNullOrWhiteSpace(ipProperties.DomainName) ? ipProperties.HostName : string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName);
        }

        PrinterAgent printeragent = new PrinterAgent();*/
        public async Task wsRequestHandler(System.Net.WebSockets.Managed.ClientWebSocket clientWebSocket, ArraySegment<byte> bytesReceived, WebSocketReceiveResult result)
        {
            processMessage(clientWebSocket, bytesReceived, result);
        }

        private async Task processMessage(System.Net.WebSockets.Managed.ClientWebSocket clientWebSocket, ArraySegment<byte> bytesReceived, WebSocketReceiveResult result)
        {
            String response = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);

            Console.WriteLine(response);

            //dynamic message = JsonConvert.DeserializeObject<dynamic>(response);
            dynamic message = JsonConvert.DeserializeObject<ExpandoObject>(response);

            Console.WriteLine(message.type);

            if(message.type == "request")
            {
                if (message.body.path == "/register")
                {
                    ar.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/printer/driver/list")
                {
                    //Console.WriteLine("got to driver handler");
                    qr.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/printer/port/create")
                {
                    ar.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/printer/port/list")
                {
                    qr.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/printer/queue/create")
                {
                    ar.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/printer/queue/delete")
                {
                    ar.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/printer/queue/flush")
                {
                    ar.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/printer/queue/getconfig")
                {
                    ar.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/printer/queue/list")
                {
                    qr.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/printer/queue/set")
                {
                    ar.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/printer/queue/setconfig")
                {
                    ar.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/printer/queue/testpage")
                {
                    ar.ProcessResponse(clientWebSocket, message);
                }
                else
                {
                    //do nothing
                }
            }
            else if (message.type == "response")
            {

            }
            else if(message.type == "ping")
            {
                
            }
            else
            {
                //do nothing
            }
        }
    }
}
