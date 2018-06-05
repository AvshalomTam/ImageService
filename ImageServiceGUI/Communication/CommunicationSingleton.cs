using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json.Linq;
using ImageServiceGUI.Enums;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Configuration;

namespace ImageServiceGUI.Communication
{
    class CommunicationSingleton
    {
        private static CommunicationSingleton instance;
        public event EventHandler<MessageEventArgs> msgReceived;
        private NetworkStream stream;
        private BinaryWriter writer;
        private BinaryReader reader;
        private TcpClient client = null;

        private CommunicationSingleton() {}

        public static CommunicationSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CommunicationSingleton();
                }
                return instance;
            }
        }

        /// <summary>
        /// Connects to service.
        /// </summary>
        /// <returns></returns>
        public int connectToService()
        {
            // create TCP connection
            string IP = ConfigurationManager.AppSettings["IP"];
            int port;
            if (!Int32.TryParse(ConfigurationManager.AppSettings["port"], out port))
                port = 8080;
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IP), port);
            client = new TcpClient();
            try
            {
                client.Connect(ep);
            }
            catch
            {
                return -1;
            }
            this.stream = client.GetStream();
            this.writer = new BinaryWriter(stream);
            this.reader = new BinaryReader(stream);

            // call read function in seperate thread
            Task task = new Task(() =>
            {
                readFromService();
            });
            task.Start();
            return 0;
        }

        /// <summary>
        /// Writes to service.
        /// </summary>
        /// <param name="message">The message.</param>
        public void writeToService(string message)
        {
            this.writer.Write(message);                        
        }

        /// <summary>
        /// Reads from service.
        /// </summary>
        public void readFromService()
        {
            // looping infinetly
            string msg;
            while (true)
            {
                try
                {
                    msg = this.reader.ReadString();
                    msgReceived?.Invoke(this, new MessageEventArgs(msg));
                }
                catch (IOException e)
                {
                    closeService();
                    return;
                }
            }                         
        }

        public void closeService()
        {
            this.client?.Close();
        }
    }
}