using ImageService.Controller;
using ImageService.ImageService.Configuration;
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
        private ConfigManager CM;
        private ServerEnum server;

        public TcpServer(IClientHandler handler, ConfigManager manager, ServerEnum server)
        {
            this.handler = handler;
            this.CM = manager;
            this.server = server;
        }

        /// <summary>
        /// Starts listening to clients.
        /// </summary>
        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(this.CM.GetIp(this.server), this.CM.GetPort(this.server));
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
