using ImageService.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication
{
    class TcpApp
    {
        private TcpListener listener;
        private AppHandler handler;
        private ILoggingService logger;

        public TcpApp(AppHandler h, ILoggingService logger)
        {
            this.handler = h;
            this.logger = logger;
        }

        public void start()
        {
            string IP = ConfigurationManager.AppSettings["IP"];
            int port;
            if (!Int32.TryParse(ConfigurationManager.AppSettings["app_port"], out port))
                port = 7777;
            //IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IP), port);
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 45267);
            listener = new TcpListener(ep);

            listener.Start();
            // waiting for connections
            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        logger.Log("Client accepted", Logging.Modal.MessageTypeEnum.INFO);
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
