using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace ImageServiceWeb.Communication
{
    public class CommunicationSingleton
    {
        private static CommunicationSingleton instance;
        public event EventHandler<MessageEventArgs> msgReceived;
        private NetworkStream stream;
        private BinaryWriter writer;
        private BinaryReader reader;
        private TcpClient client = null;

        private CommunicationSingleton()
        {
            connectToService();
        }

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
            getIpPort(out string IP, out int port);
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

        public void getIpPort(out string IP, out int port)
        {
            IP = "";
            port = 0;
            XmlReader reader = XmlReader.Create(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/TcpConnection.xml"));
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name.ToString())
                    {
                        case "IP":
                            IP = reader.ReadString();
                            break;
                        case "port":
                            if (!Int32.TryParse(reader.ReadString(), out port))
                                port = 8080;
                            break;                        
                    }
                }
            }
            reader.Close();
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