using ImageService.Controller;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication
{
    class TcpServer
    {
        private TcpListener listener;
        private IClientHandler handler;

        public TcpServer(IClientHandler handler)
        {
            this.handler = handler;
        }

        /// <summary>
        /// Starts listening to clients.
        /// </summary>
        public void Start()
        {
            string IP = ConfigurationManager.AppSettings["IP"];
            int port;
            if (!Int32.TryParse(ConfigurationManager.AppSettings["port"], out port))
                port = 8080;
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IP), port);
            listener = new TcpListener(ep);

            listener.Start();
            // waiting for connections
            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        handler.HandleClient(client);               
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
            });
            task.Start();            
        }

        /// <summary>
        /// Stops listening.
        /// </summary>
        public void Stop()
        {
            listener.Stop();
        }
    }
}
