using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;

namespace PrintManagement.wslib
{
    public class pendingResponse
    {
        public System.Net.WebSockets.Managed.ClientWebSocket ws { get; set; }
        public ArraySegment<byte> message { get; set; }
    }
    class responder
    {
        private static List<pendingResponse> sendqueue = new List<pendingResponse>();
        private static bool sending = false;
        public async Task Send(System.Net.WebSockets.Managed.ClientWebSocket ws, ArraySegment<byte> message)
        {
            pendingResponse pr = new pendingResponse();
            pr.ws = ws;
            pr.message = message;
            sendqueue.Add(pr);
            //Console.WriteLine("Send queue is now " + sendqueue.Count);
            if (sending == false)
            {
                await sendHandler();
            }
            return;
            /*await ws.SendAsync(
                message,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );*/
        }

        private async Task sendHandler()
        {
            try {
                sending = true;
                for (int i = sendqueue.Count - 1; i >= 0; i--)
                {
                    //Thread.Sleep(4000);
                    //Console.WriteLine("sending message");
                    await sendqueue[i].ws.SendAsync(
                        sendqueue[i].message,
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                    Console.WriteLine(DateTime.Now.ToString() + " Removing index:");
                    Console.WriteLine(i);
                    sendqueue.RemoveAt(i);
                }
                sending = false;
                //Console.WriteLine("Send queue is now " + sendqueue.Count);
            }
            catch (Exception e)
            {
                sending = false;
                errorlog el = new errorlog();
                el.write(e.ToString(), Environment.StackTrace, "error");
            }
        }
    }
}
