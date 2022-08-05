using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Collections;
using Newtonsoft.Json;
using System.Threading;
using System.Net.NetworkInformation;

namespace PrintManagement.responderlib
{
    class RegisterResponse
    {
        private wslib.responder wsresponser = new wslib.responder();
        private static string GetLocalhostFqdn()
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            return string.IsNullOrWhiteSpace(ipProperties.DomainName) ? ipProperties.HostName : string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName);
        }
        public async Task InventoryResponse(System.Net.WebSockets.Managed.ClientWebSocket ws, dynamic rm)
        {
            ResponseMessage resp = new ResponseMessage();

            resp.id = rm.id;
            resp.type = "response";
            resp.body = GetLocalhostFqdn();

            string jsonresp = JsonConvert.SerializeObject(resp);

            /*ArraySegment<byte> bytesToSend = new ArraySegment<byte>(
                 Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resp))
             );*/

            /*byte[] compressedbytes = Compress(Encoding.UTF8.GetBytes(jsonresp));

            string s3 = Convert.ToBase64String(compressedbytes, Base64FormattingOptions.InsertLineBreaks);*/

            //string s3 = CompressString(jsonresp);

            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(
                 Encoding.UTF8.GetBytes(jsonresp)
             );

            /*await ws.SendAsync(
                 bytesToSend,
                 WebSocketMessageType.Text,
                 true,
                 CancellationToken.None
             );*/
            try
            {
                await wsresponser.Send(ws, bytesToSend);
            }
            catch (Exception e)
            {
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");
            }
        }
    }
}
