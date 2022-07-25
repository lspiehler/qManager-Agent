using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrintManagement
{
    public class DELETE_HttpServer
    {
        public int Port = 8080;

        private HttpListener _listener;

        public void Start()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://*:" + Port.ToString() + "/");
            _listener.Start();
            Receive();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        private void Receive()
        {
            _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
        }

        private void ListenerCallback(IAsyncResult result)
        {
            if (_listener.IsListening)
            {
                var context = _listener.EndGetContext(result);
                var request = context.Request;

                // do something with the request
                Console.WriteLine($"{request.Url}");

                var response = context.Response;
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/plain";
                response.OutputStream.Write(new byte[] { }, 0, 0);
                response.OutputStream.Close();

                Receive();
            }
        }
    }
}
