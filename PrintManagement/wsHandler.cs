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
        responderlib.CachedResponse cr = new responderlib.CachedResponse();
        responderlib.RegisterResponse rr = new responderlib.RegisterResponse();

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
                    rr.InventoryResponse(clientWebSocket, message);
                    /*ResponseMessage resp = new ResponseMessage();

                    resp.id = message.id;
                    resp.type = "response";
                    resp.body = GetLocalhostFqdn();

                    string jsonresp = JsonConvert.SerializeObject(resp);*/

                    /*ArraySegment<byte> bytesToSend = new ArraySegment<byte>(
                         Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resp))
                     );*/

                    /*byte[] compressedbytes = Compress(Encoding.UTF8.GetBytes(jsonresp));

                    string s3 = Convert.ToBase64String(compressedbytes, Base64FormattingOptions.InsertLineBreaks);*/

                    //string s3 = CompressString(jsonresp);

                    /*ArraySegment<byte> bytesToSend = new ArraySegment<byte>(
                         Encoding.UTF8.GetBytes(jsonresp)
                     );

                    await clientWebSocket.SendAsync(
                         bytesToSend,
                         WebSocketMessageType.Text,
                         true,
                         CancellationToken.None
                     );*/

                }
                else if (message.body.path == "/printer/queue/list")
                {
                    cr.ProcessResponse(clientWebSocket, message);

                    /*PMPrintQueues pmprintqueues = new PMPrintQueues();

                    Hashtable myPrintQueues = pmprintqueues.getQueues();

                    HashTableResponse resp = new HashTableResponse();
                    HashTableResponseBody body = new HashTableResponseBody();

                    body.result = "success";
                    body.message = null;
                    body.data = myPrintQueues;

                    resp.id = message.id;
                    resp.type = "response";
                    resp.body = body;

                    string jsonresp = JsonConvert.SerializeObject(resp);

                    Console.WriteLine(jsonresp);*/

                    /*ArraySegment<byte> bytesToSend = new ArraySegment<byte>(
                            Encoding.UTF8.GetBytes(jsonresp)
                        );*/

                    //byte[] compressedbytes = Compress(Encoding.UTF8.GetBytes(jsonresp));

                    //string s3 = Convert.ToBase64String(compressedbytes, Base64FormattingOptions.InsertLineBreaks);

                    /*ArraySegment<byte> bytesToSend = new ArraySegment<byte>(
                            Encoding.UTF8.GetBytes(jsonresp)
                        );

                    await clientWebSocket.SendAsync(
                            bytesToSend,
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                        );*/
                }
                else if (message.body.path == "/printer/driver/list")
                {
                    //Console.WriteLine("got to driver handler");
                    cr.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/printer/port/list")
                {
                    cr.ProcessResponse(clientWebSocket, message);
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
